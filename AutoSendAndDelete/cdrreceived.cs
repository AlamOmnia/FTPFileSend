//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoSendAndDelete
{
    using System;
    using System.Collections.Generic;
    
    public partial class cdrreceived
    {
        public long idCDRReceived { get; set; }
        public string FileName { get; set; }
        public long FileSerialNumber { get; set; }
        public string FullPathLocal { get; set; }
        public Nullable<int> idSwitch { get; set; }
        public Nullable<System.DateTime> ReceiveTimeServer1 { get; set; }
        public Nullable<System.DateTime> ReceiveTimeServer2 { get; set; }
        public Nullable<int> Backedup { get; set; }
        public Nullable<int> Backedup1 { get; set; }
        public Nullable<int> Backedup2 { get; set; }
        public Nullable<sbyte> DecodedFlag { get; set; }
        public Nullable<sbyte> DecodingServer { get; set; }
        public string DecodedFileLocation { get; set; }
    }
}
