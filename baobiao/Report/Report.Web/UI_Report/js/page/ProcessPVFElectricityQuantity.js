


$(function () {
    InitDate();
    InitializeGrid("first");
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

//function loadGridData(myLoadType, organizationId, startDate, endDate) {
//    //parent.$.messager.progress({ text: '数据加载中....' });
//    var m_MsgData;
//    $.ajax({
//        type: "POST",
//        url: "ProcessPVFElectricityQuantity.aspx/GetReportData",
//        data: '{organizationId: "' + organizationId + '", startDate: "' + startDate + '", endDate: "' + endDate + '"}',
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (msg) {
//            m_MsgData = jQuery.parseJSON(msg.d);
//            InitializeGrid(myLoadType,m_MsgData);
//           // myMergeCell("gridMain_ReportTemplate", "Name");
//        },
//        error: handleError
//    });
//}

function handleError() {
    $('#gridMain_ReportTemplate').treegrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

function QueryReportFun() {
    var organizationID = $('#organizationId').val();
    var startDate = $('#startDate').datetimespinner('getValue');//开始时间
    var endDate = $('#endDate').datetimespinner('getValue');//结束时间
    if (organizationID == "" || startDate == "" || endDate == "") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }
    if (startDate > endDate) {
        $.messager.alert('警告', '结束时间不能大于开始时间！');
        return;
    }
    //loadGridData('first', organizationID, startDate, endDate);
    $.ajax({
        type: "POST",
        url: "ProcessPVFElectricityQuantity.aspx/GetReportData",
        data: '{organizationId: "' + organizationID + '", startDate: "' + startDate + '", endDate: "' + endDate + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            InitializeGrid("last", m_MsgData);
            // myMergeCell("gridMain_ReportTemplate", "Name");
        },
        error: handleError
    });
}

function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
}

function InitializeGrid(myLoadType,myData) {
    if (myLoadType == 'first') {
        $('#gridMain_ReportTemplate').treegrid({
            fit:true,
            title: '',
            data: myData,
            dataType: "json",
            striped: true,
            rownumbers: true,
            singleSelect: true,
            toolbar: '#toolbar_ReportTemplate',
            idField: 'id',
            treeField: 'Name'
        });
    }
    else {
        $('#gridMain_ReportTemplate').treegrid("loadData", myData);            
    }
}

function RefreshFun() {
    QueryReportFun();
}
//合并单元格
function myMergeCell(myDatagridId, columnName) {
    var myDatagrid = $('#' + myDatagridId);
    var merges = [];
    var myDatas = myDatagrid.datagrid('getData');
    var myRows = myDatas["rows"];
    var length = myRows.length;
    var beforeValue;
    //参数
    var count = 0;//merges数组个数
    var rowspan = 0;
    var index = 0;
    for (var i = 0; i < length; i++) {
        var currentValue = myRows[i][columnName];
        //第一个要特殊处理
        if (i == 0) {
            beforeValue = currentValue;
        }
        //前一行和后一行相同时累加数加一
        if (currentValue == beforeValue) {
            rowspan++;
        }
        else {
            //当rowspan为1时不用合并单元格
            if (rowspan > 1) {
                merges.push({ "rowspan": rowspan, "index": index });
            }
            beforeValue = currentValue;
            index = i;
            //初始化rowspan
            rowspan = 1;
        }
        //最后一个也要特殊处理
        if ((length - 1) == i && rowspan > 1) {
            merges.push({ "rowspan": rowspan, "index": index });
        }
    }
    for (var i = 0; i < merges.length; i++) {
        myDatagrid.datagrid('mergeCells', {
            index: merges[i].index,
            field: columnName,
            rowspan: merges[i].rowspan
        });
    }
}