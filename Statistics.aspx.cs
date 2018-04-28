using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTOHELPER;
using System.Globalization;

public partial class Statistics : GTOHelper
{
	protected DateTime Date
	{
		get
		{
			if (Request.Params["date"] == null)
				return DateTime.Today;
			return DateTime.ParseExact(Request.Params["date"].ToString(), "ddMMyyyy", CultureInfo.InvariantCulture);
		}
	}
	protected string totalSQL
	{
		get
		{
			return
				"select 'created' as work_type, sum(createdh) as total from (" + BaseSQL + ") tbl1 union select 'finished' as work_type, sum(finishedh)  as total from (" + BaseSQL + ") tbl2";
		}
	}
	protected string totalSQLNum
	{
		get
		{
			return
				"select 'created' as work_type, sum(created) as total from (" + BaseSQL + ") tbl1 union select 'finished' as work_type, sum(finished)  as total from (" + BaseSQL + ") tbl2";
		}
	}
	protected string BaseSQL
	{
		get
		{
			return string.Format(
				@"select max(tbl.urs) urs, sum(created*est) createdh, sum(finished*est) finishedh, sum(created) created, SUM(finished) as finished from
					(SELECT
						D.defectNum,
						(select TOP 1 e.TimeSpent from [tt_res].[dbo].DEFECTEVTS E where e.EvtDefID = 2 And e.ParentID = d.idRecord order by e.OrderNum desc) as est
						,RTRIM(D.USR) as urs
						,CASE WHEN DATEPART(m, d.dateCreate) = {0} AND DATEPART(yyyy, d.dateCreate) = {1} THEN 1 ELSE 0 END AS created
						,CASE WHEN DATEPART(m, d.dateEnter) = {0} AND DATEPART(yyyy, d.dateEnter) = {1}  and (d.idDisposit = 3 or d.idDisposit = 5) THEN 1 ELSE 0 END AS finished
						,(select c.Descriptor from [tt_res].[dbo].FLDCOMP c where c.idRecord = d.idCompon) comp
					FROM [tt_res].[dbo].[DEFECTS] D
					WHERE 
					((d.idDisposit = 3 or d.idDisposit = 5) and
					(DATEPART(m, d.dateEnter) = {0} AND DATEPART(yyyy, d.dateEnter) = {1}))
					or (DATEPART(m, d.dateCreate) = {0} AND DATEPART(yyyy, d.dateCreate) = {1})
					) tbl
					where 
					urs in (select PERSONS.WORK_EMAIL from PERSONS where PERSONS.LEVEL_ID = (select L.LEVEL_ID from LEVELS L where L.LEVEL_NAME = 'programmer'))
					and comp not like '%vacation%'
					group by tbl.urs "
				 , Date.Month, Date.Year);

		}
	}
	protected string PersonalSQL
	{
		get
		{
			return BaseSQL + " Order By 1";
		}
	}
	protected void Page_Load(object sender, EventArgs e)
	{
			UpdateSQLs();
	}
	protected void UpdateSQLs()
	{
		SqlDataSource1.SelectCommand = totalSQL;
		SqlDataSource2.SelectCommand = PersonalSQL;
		SqlDataSource3.SelectCommand = totalSQLNum;
	}
}