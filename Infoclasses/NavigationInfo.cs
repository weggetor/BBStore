using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for NavigationInfo
/// </summary>
public class NavigationInfo
{
	public enum NavigationState
	{
	    Empty,
        Set,
        Active
	}

    public NavigationState State { get; set; }
    public string Caption { get; set; }
    public string Action { get; set; }
    public bool Enabled { get; set; }
	public string Url { get; set; }

    public NavigationInfo()
	{
	}
  
}