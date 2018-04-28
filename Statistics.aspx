<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Statistics.aspx.cs" Inherits="Statistics" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Statistics</title>
	<script type="text/javascript" src="Scripts/jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery-ui.min.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="Scripts/Common.js"></script>
	<script type="text/javascript" src="Scripts/Statistics.js"></script>
	<link rel="stylesheet" type="text/css" href="Styles/MonthSelector.css" />
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="select 'Created' as work_type, SUM(est) as total from 
(
	SELECT
	(select TOP 1 e.TimeSpent from [tt_res].[dbo].DEFECTEVTS E where e.EvtDefID = 2 And e.ParentID = d.idRecord order by e.OrderNum desc) as est
	FROM [tt_res].[dbo].[DEFECTS] D
	WHERE 
	DATEPART(m, d.dateCreate) = 6 AND DATEPART(yyyy, d.dateCreate) = 2014
) tbl
union all
select 'Finished' as work_type, SUM(est) as total from 
(
	SELECT
	(select TOP 1 e.TimeSpent from [tt_res].[dbo].DEFECTEVTS E where e.EvtDefID = 2 And e.ParentID = d.idRecord order by e.OrderNum desc) as est
	FROM [tt_res].[dbo].[DEFECTS] D
	WHERE 
	(d.idDisposit = 3 or d.idDisposit = 5) and
	(DATEPART(m, d.dateEnter) = 6 AND DATEPART(yyyy, d.dateEnter) = 2014)
) tbl

"></asp:SqlDataSource>

			<asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="select max(tbl.urs) urs, sum(created*est) createdh, sum(finished*est) finishedh, sum(created) created, SUM(finished) as finished from
(SELECT
	D.defectNum,
	(select TOP 1 e.TimeSpent from [tt_res].[dbo].DEFECTEVTS E where e.EvtDefID = 2 And e.ParentID = d.idRecord order by e.OrderNum desc) as est
	,(select u.EMailAddr from [tt_res].[dbo].USERS u where u.idRecord = 
		RTRIM(D.USR) as urs
	,CASE WHEN DATEPART(m, d.dateCreate) = 6 AND DATEPART(yyyy, d.dateCreate) = 2014 THEN 1 ELSE 0 END AS created
	,CASE WHEN DATEPART(m, d.dateEnter) = 6 AND DATEPART(yyyy, d.dateEnter) = 2014  and (d.idDisposit = 3 or d.idDisposit = 5) THEN 1 ELSE 0 END AS finished
	,(select c.Descriptor from [tt_res].[dbo].FLDCOMP c where c.idRecord = d.idCompon) comp
FROM [tt_res].[dbo].[DEFECTS] D
WHERE 
((d.idDisposit = 3 or d.idDisposit = 5) and
(DATEPART(m, d.dateEnter) = 6 AND DATEPART(yyyy, d.dateEnter) = 2014))
or (DATEPART(m, d.dateCreate) = 6 AND DATEPART(yyyy, d.dateCreate) = 2014)
) tbl
where 
urs in (select PERSONS.WORK_EMAIL from PERSONS where PERSONS.LEVEL_ID = (select L.LEVEL_ID from LEVELS L where L.LEVEL_NAME = 'programmer'))
and comp not like '%vacation%'
group by tbl.urs
order by 1"></asp:SqlDataSource>

			<asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="select 'Created' as work_type, count(*) as total from 
            (
	            SELECT
	            (select TOP 1 e.TimeSpent from [tt_res].[dbo].DEFECTEVTS E where e.EvtDefID = 2 And e.ParentID = d.idRecord order by e.OrderNum desc) as est
	            FROM [tt_res].[dbo].[DEFECTS] D
	            WHERE 
	            DATEPART(m, d.dateCreate) = 8 AND DATEPART(yyyy, d.dateCreate) = 2014
            ) tbl
            union all
            select 'Finished' as work_type, count(*) as total from 
            (
	            SELECT
	            (select TOP 1 e.TimeSpent from [tt_res].[dbo].DEFECTEVTS E where e.EvtDefID = 2 And e.ParentID = d.idRecord order by e.OrderNum desc) as est
	            FROM [tt_res].[dbo].[DEFECTS] D
	            WHERE 
	            (d.idDisposit = 3 or d.idDisposit = 5) and
	            (DATEPART(m, d.dateEnter) = 8 AND DATEPART(yyyy, d.dateEnter) = 2014)
            ) tbl"></asp:SqlDataSource>

			<label>Date:</label>
			<input id="startdate" />

			<br />
			<span>Total Hours Per Month</span>
			<br />
			<asp:Chart ID="Chart2" runat="server" DataSourceID="SqlDataSource1" Height="444px" Width="444px" ImageLocation="~/TEMPIMAGES/ChartPic_#SEQ(300,3)">
				<Series>
					<asp:Series ChartType="Pie" IsValueShownAsLabel="True" Name="Series1" XValueMember="work_type" YValueMembers="total" ChartArea="ChartArea1" Legend="Legend1">
					</asp:Series>
				</Series>
				<ChartAreas>
					<asp:ChartArea Name="ChartArea1">
						<Area3DStyle Enable3D="True" LightStyle="Realistic" />
					</asp:ChartArea>
				</ChartAreas>
				<Legends>
					<asp:Legend Docking="Bottom" Name="Legend1">
					</asp:Legend>
				</Legends>
			</asp:Chart>

			<asp:Chart ID="Chart5" runat="server" DataSourceID="SqlDataSource2" Height="444px" Width="555px">
				<Series>
					<asp:Series ChartType="StackedColumn" Legend="Legend1" Name="Created" XValueMember="urs" YValueMembers="createdh">
					</asp:Series>
					<asp:Series ChartArea="ChartArea1" Legend="Legend1" Name="Finished" XValueMember="urs" YValueMembers="finishedh" ChartType="StackedColumn">
					</asp:Series>
				</Series>
				<ChartAreas>
					<asp:ChartArea Name="ChartArea1">
						<AxisX Interval="1">
						</AxisX>
					</asp:ChartArea>
				</ChartAreas>
				<Legends>
					<asp:Legend Name="Legend1">
					</asp:Legend>
				</Legends>
			</asp:Chart>

			<br />
			<span>Tasks Per Month</span><br />
			<asp:Chart ID="Chart4" runat="server" DataSourceID="SqlDataSource3" Height="444px" Width="444px" ImageLocation="~/TEMPIMAGES/ChartPic_#SEQ(300,3)">
				<Series>
					<asp:Series ChartType="Pie" IsValueShownAsLabel="True" Name="Series1" ChartArea="ChartArea1" Legend="Legend1" XValueMember="work_type" YValueMembers="total">
					</asp:Series>
				</Series>
				<ChartAreas>
					<asp:ChartArea Name="ChartArea1">
						<Area3DStyle Enable3D="True" LightStyle="Realistic" />
					</asp:ChartArea>
				</ChartAreas>
				<Legends>
					<asp:Legend Docking="Bottom" Name="Legend1">
					</asp:Legend>
				</Legends>
			</asp:Chart>

			<asp:Chart ID="Chart6" runat="server" DataSourceID="SqlDataSource2" Height="444px" Width="555px">
				<Series>
					<asp:Series ChartType="StackedColumn" Legend="Legend1" Name="Created" XValueMember="urs" YValueMembers="created">
					</asp:Series>
					<asp:Series ChartArea="ChartArea1" Legend="Legend1" Name="Finished" XValueMember="urs" YValueMembers="finished" ChartType="StackedColumn">
					</asp:Series>
				</Series>
				<ChartAreas>
					<asp:ChartArea Name="ChartArea1">
						<AxisX Interval="1">
						</AxisX>
					</asp:ChartArea>
				</ChartAreas>
				<Legends>
					<asp:Legend Name="Legend1">
					</asp:Legend>
				</Legends>
			</asp:Chart>

		</div>
	</form>
</body>
</html>
