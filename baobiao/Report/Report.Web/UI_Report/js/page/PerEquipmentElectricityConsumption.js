
var myDataGridObject = new Object();// $('#gridMain_ReportTemplate');
$(function () {
    InitDate();
    loadDataGrid("first");
    myDataGridObject = $('#gridMain_ReportTemplate');
});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth()) + '-' + beforeDate.getDate();
    $('#startDate').datebox('setValue', beforeString);
    $('#endDate').datebox('setValue', nowString);
}

function loadGridData(myLoadType, organizationId, startDate, endDate) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "PerEquipmentElectricityConsumption.aspx/GetReportData",
        data: '{organizationId: "' + organizationId + '", startDate: "' + startDate + '", endDate: "' + endDate  + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            loadDataGrid("last", m_MsgData);
            //合并单元格
           // myMergeCell("gridMain_ReportTemplate", "ProductLineName");
        },
        error: handleError
    });
}

function handleError() {
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function QueryReportFun() {
    editIndex = undefined;
    var organizationID = $('#organizationId').val();
    var startDate = $('#startDate').datetimebox('getValue');//开始时间
    var endDate = $('#endDate').datetimebox('getValue');//结束时间
    if (organizationID == "" || startDate == "" || endDate == "") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }
    if (startDate > endDate) {
        $.messager.alert('警告', '结束时间不能大于开始时间！');
        return;
    }
    loadGridData('last', organizationID, startDate, endDate);
}

function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
}

function loadDataGrid(type, myData) {
    if ("first" == type) {
        $('#gridMain_ReportTemplate').datagrid({           
            toolbar: "#toolbar_ReportTemplate",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            fit:true,
            data: []
        })
    }
    else {
        $('#gridMain_ReportTemplate').datagrid({ columns: myData['columns'] });
        $('#gridMain_ReportTemplate').datagrid("loadData", myData["rows"]);
    }
}

function RefreshFun() {
    QueryReportFun();
}

function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = "Parameter1";
    var m_Parameter2 = "Parameter2";

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "PerEquipmentElectricityConsumption.aspx");

    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();

    /*
    var m_Parmaters = { "myFunctionName": m_FunctionName, "myParameter1": m_Parameter1, "myParameter2": m_Parameter2 };
    $.ajax({
        type: "POST",
        url: "Report_Example.aspx",
        data: m_Parmater,                       //'myFunctionName=' + m_FunctionName + '&myParameter1=' + m_Parameter1 + '&myParameter2=' + m_Parameter2,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d == "1") {
                alert("导出成功!");
            }
            else{
                alert(msg.d);
            }
        }
    });
    */
}