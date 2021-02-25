namespace Sobees.Library.BFacebookLibV1.Cls
{
  ///<summary>
  ///</summary>
  public class FacebookVideoUploadLimits
  {
    ///<summary>
    ///</summary>
    public FacebookVideoUploadLimits()
    {
    }

    ///<summary>
    ///</summary>
    ///<param name="length"></param>
    ///<param name="size"></param>
    public FacebookVideoUploadLimits(int length, int size)
    {
      Length = length;
      Size = size;
    }

    ///<summary>
    ///</summary>
    public int Length { get; set; }

    ///<summary>
    ///</summary>
    public int Size { get; set; }
  }
}