<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcessPVFElectricityConsumption.aspx.cs" Inherits="Report.Web.UI_Report.ProcessPVFElectricityConsumption" %>

<%@ Register Src="../UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagName="OrganisationTree" TagPrefix="uc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>峰谷平电耗报表</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/UI_Report/js/page/ProcessPVFElectricityConsumption.js"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'west',split:true" style="width:230px;">
            <uc1:OrganisationTree ID="OrganisationTree_ProductionLine" runat="server" />
        </div>
        <!-- 图表开始 -->
        <div id="toolbar_ReportTemplate" style="display: none;">
            <table>
                <tr>
	                <td>
		                <table>
			                <tr>
				                <td>生产线：</td>
		                        <td><input id="productLineName" class="easyui-textbox" style="width:180px;" readonly="true" /><input id="organizationId" readonly="true" style="display:none;"/></td>
				                <td>开始时间：</td>
				                <td>
                                    <%--<input id="datetime" class="easyui-datetimespinner" value="6/24/2014" data-options="formatter:formatter2,parser:parser2,selections:[[0,4],[5,7]]" style="width:180px;" />--%>
                                    <input id="startDate" type="text" class="easyui-datebox" required="required" style="width:100px;"/>
				                </td>
                                <td>结束时间：</td>
				                <td>
                                    <%--<input id="datetime" class="easyui-datetimespinner" value="6/24/2014" data-options="formatter:formatter2,parser:parser2,selections:[[0,4],[5,7]]" style="width:180px;" />--%>
                                    <input id="endDate" type="text" class="easyui-datebox" required="required" style="width:100px;"/>
				                </td>
				                <td><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" 
                                        onclick="QueryReportFun();">查询</a>
                                </td>
			                </tr>
			            </table>
		            </td>
	            </tr>
	            <tr>
	                <td>
		                <table>

	                        <tr>
                                <td>
                                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-reload',plain:true" onclick="RefreshFun();">刷新</a>
                                </td>
                                <!--<td><div class="datagrid-btn-separator"></div>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                </td>
                                <td><a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a>
                                </td>-->
                            </tr>
                        </table>
		            </td>
                </tr>
	        </table>
        </div>

        <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:false, split:false">
            <table id="gridMain_ReportTemplate" class="easyui-datagrid" data-options="toolbar:'#toolbar_ReportTemplate',rownumbers:true,singleSelect:true" title="">
		        <thead>
			        <tr>
                        <th data-options="field:'Name',width:80">产线名</th>
                        <th data-options="field:'VariableName',width:150">项目</th>
                        <th data-options="field:'PeakB',width:80">峰期</th>
                        <th data-options="field:'MorePeakB',width:80">尖峰期</th>
                        <th data-options="field:'ValleyB',width:80">谷期</th>
                        <th data-options="field:'FlatB',width:80">平期</th>
                        <th data-options="field:'TotalPeakValleyFlatB',width:100">合计</th>
			        </tr>
		        </thead>
            </table>
        </div>
        <!-- 图表结束 -->
    </div>

    <form id="form_Main" runat="server"></form>

</body>
</html>

