using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BLinkedInLib
{
  [DataContract]
  public class LinkedInIndustryCode
  {
    public LinkedInIndustryCode(int code, string group, string description)
    {
      Code = code;
      Group = group;
      Description = description;
    }

#if !SILVERLIGHT
    [DataMember]
#endif
      public int Code { get; set; }

#if !SILVERLIGHT
    [DataMember]
#endif
      public string Group { get; set; }

#if !SILVERLIGHT
    [DataMember]
#endif
      public string Description { get; set; }

    public static List<LinkedInIndustryCode> GetAllCode()
    {
      var lst = new List<LinkedInIndustryCode>
                  {
                    new LinkedInIndustryCode(47, "corp fin", "Accounting"),
                    new LinkedInIndustryCode(94, "man tech tran", "Airlines/Aviation"),
                    new LinkedInIndustryCode(120, "leg org", "Alternative Dispute Resolution"),
                    new LinkedInIndustryCode(125, "hlth", "Alternative Medicine"),
                    new LinkedInIndustryCode(127, "art med", "Animation"),
                    new LinkedInIndustryCode(19, "good", "Apparel & Fashion"),
                    new LinkedInIndustryCode(50, "cons", "Architecture & Planning"),
                    new LinkedInIndustryCode(111, "art med rec", "Arts and Crafts"),
                    new LinkedInIndustryCode(53, "man", "Automotive"),
                    new LinkedInIndustryCode(52, "gov man", "Aviation & Aerospace"),
                    new LinkedInIndustryCode(41, "fin", "Banking"),
                    new LinkedInIndustryCode(12, "gov hlth tech", "Biotechnology"),
                    new LinkedInIndustryCode(36, "med rec", "Broadcast Media"),
                    new LinkedInIndustryCode(49, "cons", "Building Materials"),
                    new LinkedInIndustryCode(138, "corp man", "Business Supplies and Equipment"),
                    new LinkedInIndustryCode(129, "fin", "Capital Markets"),
                    new LinkedInIndustryCode(54, "man", "Chemicals"),
                    new LinkedInIndustryCode(90, "org serv", "Civic & Social Organization"),
                    new LinkedInIndustryCode(51, "cons gov", "Civil Engineering"),
                    new LinkedInIndustryCode(128, "cons corp fin", "Commercial Real Estate"),
                    new LinkedInIndustryCode(118, "tech", "Computer & Network Security"),
                    new LinkedInIndustryCode(109, "med rec", "Computer Games"),
                    new LinkedInIndustryCode(3, "tech", "Computer Hardware"),
                    new LinkedInIndustryCode(5, "tech", "Computer Networking"),
                    new LinkedInIndustryCode(4, "tech", "Computer Software"),
                    new LinkedInIndustryCode(48, "cons", "Construction"),
                    new LinkedInIndustryCode(24, "good man", "Consumer Electronics"),
                    new LinkedInIndustryCode(25, "good man", "Consumer Goods"),
                    new LinkedInIndustryCode(91, "org serv", "Consumer Services"),
                    new LinkedInIndustryCode(18, "good", "Cosmetics"),
                    new LinkedInIndustryCode(65, "agr", "Dairy"),
                    new LinkedInIndustryCode(1, "gov tech", "Defense & Space"),
                    new LinkedInIndustryCode(99, "art med", "Design"),
                    new LinkedInIndustryCode(69, "edu", "Education Management"),
                    new LinkedInIndustryCode(132, "edu org", "E-Learning"),
                    new LinkedInIndustryCode(112, "good man", "Electrical/Electronic Manufacturing"),
                    new LinkedInIndustryCode(28, "med rec", "Entertainment"),
                    new LinkedInIndustryCode(86, "org serv", "Environmental Services"),
                    new LinkedInIndustryCode(110, "corp rec serv", "Events Services"),
                    new LinkedInIndustryCode(76, "gov", "Executive Office"),
                    new LinkedInIndustryCode(122, "corp serv", "Facilities Services"),
                    new LinkedInIndustryCode(63, "agr", "Farming"),
                    new LinkedInIndustryCode(43, "fin", "Financial Services"),
                    new LinkedInIndustryCode(38, "art med rec", "Fine Art"),
                    new LinkedInIndustryCode(66, "agr", "Fishery"),
                    new LinkedInIndustryCode(34, "rec serv", "Food & Beverages"),
                    new LinkedInIndustryCode(23, "good man serv", "Food Production"),
                    new LinkedInIndustryCode(101, "org", "Fund-Raising"),
                    new LinkedInIndustryCode(26, "good man", "Furniture"),
                    new LinkedInIndustryCode(29, "rec", "Gambling & Casinos"),
                    new LinkedInIndustryCode(145, "cons man", "Glass, Ceramics & Concrete"),
                    new LinkedInIndustryCode(75, "gov", "Government Administration"),
                    new LinkedInIndustryCode(148, "gov", "Government Relations"),
                    new LinkedInIndustryCode(140, "art med", "Graphic Design"),
                    new LinkedInIndustryCode(124, "hlth rec", "Health, Wellness and Fitness"),
                    new LinkedInIndustryCode(68, "edu", "Higher Education"),
                    new LinkedInIndustryCode(14, "hlth", "Hospital & Health Care"),
                    new LinkedInIndustryCode(31, "rec serv tran", "Hospitality"),
                    new LinkedInIndustryCode(137, "corp", "Human Resources"),
                    new LinkedInIndustryCode(134, "corp good tran", "Import and Export"),
                    new LinkedInIndustryCode(88, "org serv", "Individual & Family Services"),
                    new LinkedInIndustryCode(147, "cons man", "Industrial Automation"),
                    new LinkedInIndustryCode(84, "med serv", "Information Services"),
                    new LinkedInIndustryCode(96, "tech", "Information Technology and Services"),
                    new LinkedInIndustryCode(42, "fin", "Insurance"),
                    new LinkedInIndustryCode(74, "gov", "International Affairs"),
                    new LinkedInIndustryCode(141, "gov org tran", "International Trade and Development"),
                    new LinkedInIndustryCode(6, "tech", "Internet"),
                    new LinkedInIndustryCode(45, "fin", "Investment Banking"),
                    new LinkedInIndustryCode(46, "fin", "Investment Management"),
                    new LinkedInIndustryCode(73, "gov leg", "Judiciary"),
                    new LinkedInIndustryCode(77, "gov leg", "Law Enforcement"),
                    new LinkedInIndustryCode(9, "leg", "Law Practice"),
                    new LinkedInIndustryCode(10, "leg", "Legal Services"),
                    new LinkedInIndustryCode(72, "gov leg", "Legislative Office"),
                    new LinkedInIndustryCode(30, "rec serv tran", "Leisure, Travel & Tourism"),
                    new LinkedInIndustryCode(85, "med rec serv", "Libraries"),
                    new LinkedInIndustryCode(116, "corp tran", "Logistics and Supply Chain"),
                    new LinkedInIndustryCode(143, "good", "Luxury Goods & Jewelry"),
                    new LinkedInIndustryCode(55, "man", "Machinery"),
                    new LinkedInIndustryCode(11, "corp", "Management Consulting"),
                    new LinkedInIndustryCode(95, "tran", "Maritime"),
                    new LinkedInIndustryCode(97, "corp", "Market Research"),
                    new LinkedInIndustryCode(80, "corp med", "Marketing and Advertising"),
                    new LinkedInIndustryCode(135, "cons gov man", "Mechanical or Industrial Engineering"),
                    new LinkedInIndustryCode(126, "med rec", "Media Production"),
                    new LinkedInIndustryCode(17, "hlth", "Medical Devices"),
                    new LinkedInIndustryCode(13, "hlth", "Medical Practice"),
                    new LinkedInIndustryCode(139, "hlth", "Mental Health Care"),
                    new LinkedInIndustryCode(71, "gov", "Military"),
                    new LinkedInIndustryCode(56, "man", "Mining & Metals"),
                    new LinkedInIndustryCode(35, "art med rec", "Motion Pictures and Film"),
                    new LinkedInIndustryCode(37, "art med rec", "Museums and Institutions"),
                    new LinkedInIndustryCode(115, "art rec", "Music"),
                    new LinkedInIndustryCode(114, "gov man tech", "Nanotechnology"),
                    new LinkedInIndustryCode(81, "med rec", "Newspapers"),
                    new LinkedInIndustryCode(100, "org", "Non-Profit Organization Management"),
                    new LinkedInIndustryCode(57, "man", "Oil & Energy"),
                    new LinkedInIndustryCode(113, "med", "Online Media"),
                    new LinkedInIndustryCode(123, "corp", "Outsourcing/Offshoring"),
                    new LinkedInIndustryCode(87, "serv tran", "Package/Freight Delivery"),
                    new LinkedInIndustryCode(146, "good man", "Packaging and Containers"),
                    new LinkedInIndustryCode(61, "man", "Paper & Forest Products"),
                    new LinkedInIndustryCode(39, "art med rec", "Performing Arts"),
                    new LinkedInIndustryCode(15, "hlth tech", "Pharmaceuticals"),
                    new LinkedInIndustryCode(131, "org", "Philanthropy"),
                    new LinkedInIndustryCode(136, "art med rec", "Photography"),
                    new LinkedInIndustryCode(117, "man", "Plastics"),
                    new LinkedInIndustryCode(107, "gov org", "Political Organization"),
                    new LinkedInIndustryCode(67, "edu", "Primary/Secondary Education"),
                    new LinkedInIndustryCode(83, "med rec", "Printing"),
                    new LinkedInIndustryCode(105, "corp", "Professional Training & Coaching"),
                    new LinkedInIndustryCode(102, "corp org", "Program Development"),
                    new LinkedInIndustryCode(79, "gov", "Public Policy"),
                    new LinkedInIndustryCode(98, "corp", "Public Relations and Communications"),
                    new LinkedInIndustryCode(78, "gov", "Public Safety"),
                    new LinkedInIndustryCode(82, "med rec", "Publishing"),
                    new LinkedInIndustryCode(62, "man", "Railroad Manufacture"),
                    new LinkedInIndustryCode(64, "agr", "Ranching"),
                    new LinkedInIndustryCode(44, "cons fin good", "Real Estate"),
                    new LinkedInIndustryCode(40, "rec serv", "Recreational Facilities and Services"),
                    new LinkedInIndustryCode(89, "org serv", "Religious Institutions"),
                    new LinkedInIndustryCode(144, "gov man org", "Renewables & Environment"),
                    new LinkedInIndustryCode(70, "edu gov", "Research"),
                    new LinkedInIndustryCode(32, "rec serv", "Restaurants"),
                    new LinkedInIndustryCode(27, "good man", "Retail"),
                    new LinkedInIndustryCode(121, "corp org serv", "Security and Investigations"),
                    new LinkedInIndustryCode(7, "tech", "Semiconductors"),
                    new LinkedInIndustryCode(58, "man", "Shipbuilding"),
                    new LinkedInIndustryCode(20, "good rec", "Sporting Goods"),
                    new LinkedInIndustryCode(33, "rec", "Sports"),
                    new LinkedInIndustryCode(104, "corp", "Staffing and Recruiting"),
                    new LinkedInIndustryCode(22, "good", "Supermarkets"),
                    new LinkedInIndustryCode(8, "gov tech", "Telecommunications"),
                    new LinkedInIndustryCode(60, "man", "Textiles"),
                    new LinkedInIndustryCode(130, "gov org", "Think Tanks"),
                    new LinkedInIndustryCode(21, "good", "Tobacco"),
                    new LinkedInIndustryCode(108, "corp gov serv", "Translation and Localization"),
                    new LinkedInIndustryCode(92, "tran", "Transportation/Trucking/Railroad"),
                    new LinkedInIndustryCode(59, "man", "Utilities"),
                    new LinkedInIndustryCode(106, "fin tech", "Venture Capital & Private Equity"),
                    new LinkedInIndustryCode(16, "hlth", "Veterinary"),
                    new LinkedInIndustryCode(93, "tran", "Warehousing"),
                    new LinkedInIndustryCode(133, "good", "Wholesale"),
                    new LinkedInIndustryCode(142, "good man rec", "Wine and Spirits"),
                    new LinkedInIndustryCode(119, "tech", "Wireless"),
                    new LinkedInIndustryCode(103, "art med rec", "Writing and Editing")
                  };
      return lst;
    }

    public static LinkedInIndustryCode GetIndustry(string name)
    {
      foreach (LinkedInIndustryCode industryCode in GetAllCode())
      {
        if (industryCode.Description == name)
        {
          return industryCode;
        }
      }
      return null;
    }
  }
}