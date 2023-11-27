using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Models;
[Table("tblPromotionHeader", Schema = "promo")]
public class SQLServertblPromotionHeader
{
    [Key]
    public long intPromotionId {set;get;}
    public long intPromotionTypeId {set;get;}
    public string strPromotionCode {set;get;}
    public string strPromotionName {set;get;}
    public DateTime dtePromotionStartDateTime {set;get;}
    public DateTime dtePromotionEndDateTime {set;get;}
    public string strRemarks {set;get;}
    public long intAccountId {set;get;}
    public long intBusinessUnitId {set;get;}
    public string strAttachmentLink {set;get;}
    public bool isActive {set;get;}
    public long intStatus {set;get;}
    public long intActionBy {set;get;}
    public string UserName {set;get;}
    public DateTime dteServerDateTime {set;get;}
    public DateTime dteLastActionDateTime {set; get;}
}
