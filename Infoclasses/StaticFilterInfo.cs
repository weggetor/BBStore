using System;
using System.Runtime.Serialization;

[Serializable]
[DataContract()]
public class StaticFilterInfo
{
	public StaticFilterInfo()
	{
		StaticFilterId = 0;
		PortalId = 0;
		Token = "";
		FilterCondition = "";
	}
	[DataMember()]
	public Int32 StaticFilterId { get; set; }
	[DataMember()]
	public Int32 PortalId { get; set; }
	[DataMember()]
	public string Token { get; set; }
	[DataMember()]
	public string FilterCondition { get; set; }
}
