<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainMachineElectricityConsumptionMonitor.aspx.cs" Inherits="Report.Web.UI_ReportInDCSMonitor.MainMachineElectricityConsumptionMonitor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>能耗报警查询</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="/lib/ealib/extend/jquery.PrintArea.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/extend/jquery.jqprint.js" charset="utf-8"></script>

    <script type="text/javascript" src="/UI_ReportInDCSMonitor/js/page/MainMachineElectricityConsumptionMonitor.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'north'" style="height:50px; text-align:center; vertical-align:top; font-size:20pt; font-weight:bold; padding-top:10px; font-family:SimSun;">
            <span>高压设备电耗监控</span>
        </div>
        <div data-options="region:'center', border:false, collapsible:false, split:false">
            <table id="gridMain_RealtimeReport"></table>
        </div>
    </div>
    <form id="formMain" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
