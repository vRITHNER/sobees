#region Includes

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Cls
{
  public class BindingProperties
  {
    public BindingProperties()
    {
      RelativeSourceAncestorLevel = 1;
    }

    public string SourceProperty { get; set; }
    public string ElementName { get; set; }
    public string TargetProperty { get; set; }
    public IValueConverter Converter { get; set; }
    public object ConverterParameter { get; set; }
    public bool RelativeSourceSelf { get; set; }
    public string RelativeSourceAncestorType { get; set; }
    public int RelativeSourceAncestorLevel { get; set; }
  }

  public static class BindingHelper
  {
    public static readonly DependencyProperty BindingProperty =
      DependencyProperty.RegisterAttached("Binding", typeof(List<BindingProperties>), typeof(BindingHelper),
                                          new PropertyMetadata(null, OnBinding));

    private static readonly BindingFlags dpFlags = BindingFlags.Public | BindingFlags.Static |
                                                   BindingFlags.FlattenHierarchy;

    /// <summary>
    /// A cache of relay bindings, keyed by RelayBindingKey which specifies a property of a specific 
    /// framework element.
    /// </summary>
    private static readonly Dictionary<RelayBindingKey, ValueObject> relayBindings =
      new Dictionary<RelayBindingKey, ValueObject>();

    public static List<BindingProperties> GetBinding(DependencyObject obj)
    {
      return (List<BindingProperties>)obj.GetValue(BindingProperty);
    }

    public static void SetBinding(DependencyObject obj, BindingProperties value)
    {
      var list = (List<BindingProperties>)obj.GetValue(BindingProperty);

      if (list == null)
        obj.SetValue(BindingProperty, new List<BindingProperties> { value });
      else
        list.Add(value);
    }


    /// <summary>
    /// property change event handler for BindingProperty
    /// </summary>
    private static void OnBinding(
      DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      var targetElement = depObj as FrameworkElement;

      targetElement.Loaded += TargetElement_Loaded;
    }

    private static void TargetElement_Loaded(object sender, RoutedEventArgs e)
    {
#if SILVERLIGHT
      if (DesignerProperties.IsInDesignTool)
        return;
#endif
      var targetElement = sender as FrameworkElement;

      // get the value of our attached property
      List<BindingProperties> bindingPropertiesList = GetBinding(targetElement);

      foreach (var bindingProperties in bindingPropertiesList)
      {
        if (bindingProperties.ElementName != null)
        {
          // perform our 'ElementName' lookup
          var sourceElement = targetElement.FindName(bindingProperties.ElementName) as FrameworkElement;

          // bind them
          CreateRelayBinding(targetElement, sourceElement, bindingProperties);
        }
        else if (bindingProperties.RelativeSourceSelf)
        {
          // bind an element to itself.
          CreateRelayBinding(targetElement, targetElement, bindingProperties);
        }
        else if (!string.IsNullOrEmpty(bindingProperties.RelativeSourceAncestorType))
        {
          try
          {
            // navigate up the tree to find the type
            DependencyObject currentObject = targetElement;

            int currentLevel = 0;
            while (currentLevel < bindingProperties.RelativeSourceAncestorLevel)
            {
              do
              {
                currentObject = VisualTreeHelper.GetParent(currentObject);
              } while (currentObject.GetType().Name != bindingProperties.RelativeSourceAncestorType);
              currentLevel++;
            }

            var sourceElement = currentObject as FrameworkElement;

            // bind them
            CreateRelayBinding(targetElement, sourceElement, bindingProperties);
          }
          catch (Exception ex)
          {
            TraceHelper.Trace("BindingHelper", ex);
          }
        }
      }
    }

    /// <summary>
    /// Creates a relay binding between the two given elements using the properties and converters
    /// detailed in the supplied bindingProperties.
    /// </summary>
    private static void CreateRelayBinding(FrameworkElement targetElement, FrameworkElement sourceElement,
                                           BindingProperties bindingProperties)
    {
      string sourcePropertyName = bindingProperties.SourceProperty + "Property";
      string targetPropertyName = bindingProperties.TargetProperty + "Property";

      // find the source dependency property
      FieldInfo[] sourceFields = sourceElement.GetType().GetFields(dpFlags);
      FieldInfo sourceDependencyPropertyField = sourceFields.First(i => i.Name == sourcePropertyName);
      var sourceDependencyProperty = sourceDependencyPropertyField.GetValue(null) as DependencyProperty;

      // find the target dependency property
      FieldInfo[] targetFields = targetElement.GetType().GetFields(dpFlags);
      FieldInfo targetDependencyPropertyField = targetFields.First(i => i.Name == targetPropertyName);
      var targetDependencyProperty = targetDependencyPropertyField.GetValue(null) as DependencyProperty;


      ValueObject relayObject;
      bool relayObjectBoundToSource = false;

      // create a key that identifies this source binding
      var key = new RelayBindingKey
      {
        dependencyObject = sourceDependencyProperty,
        frameworkElement = sourceElement
      };

      // do we already have a binding to this property?
      if (relayBindings.ContainsKey(key))
      {
        relayObject = relayBindings[key];
        relayObjectBoundToSource = true;
      }
      else
      {
        // create a relay binding between the two elements
        relayObject = new ValueObject();
      }


      // initialise the relay object with the source dependency property value 
      relayObject.Value = sourceElement.GetValue(sourceDependencyProperty);

      // create the binding for our target element to the relay object, this binding will
      // include the value converter
      var targetToRelay = new Binding();
      targetToRelay.Source = relayObject;
      targetToRelay.Path = new PropertyPath("Value");
      targetToRelay.Mode = BindingMode.TwoWay;
      targetToRelay.Converter = bindingProperties.Converter;
      targetToRelay.ConverterParameter = bindingProperties.ConverterParameter;

      // set the binding on our target element
      targetElement.SetBinding(targetDependencyProperty, targetToRelay);

      if (!relayObjectBoundToSource)
      {
        // create the binding for our source element to the relay object
        var sourceToRelay = new Binding();
        sourceToRelay.Source = relayObject;
        sourceToRelay.Path = new PropertyPath("Value");
        sourceToRelay.Mode = BindingMode.TwoWay;

        // set the binding on our source element
        sourceElement.SetBinding(sourceDependencyProperty, sourceToRelay);

        relayBindings.Add(key, relayObject);
      }
    }

    #region Nested type: RelayBindingKey

    private struct RelayBindingKey
    {
      public DependencyProperty dependencyObject;
      public FrameworkElement frameworkElement;
    }

    #endregion

    #region Nested type: ValueObject

    public class ValueObject : INotifyPropertyChanged
    {
      private object _value;

      public object Value
      {
        get { return _value; }
        set
        {
          _value = value;
          OnPropertyChanged("Value");
        }
      }

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion

      protected virtual void OnPropertyChanged(string propertyName)
      {
        if (PropertyChanged != null)
        {
          PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
      }
    }

    #endregion
  }
}