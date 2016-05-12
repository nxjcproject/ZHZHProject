
var myDataGridObject = new Object();// $('#gridMain_ReportTemplate');
var merges = [];
$(function () {
    InitDate();
    loadDataGrid("first");
    myDataGridObject = $('#gridMain_ReportTemplate');
});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    //nowDate.setDate(nowDate.getDate() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() /*+ " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds()*/;
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth()) + '-' + beforeDate.getDate();
    $('#startDate').datebox('setValue', beforeString);
    $('#endDate').datebox('setValue', nowString);
}

function loadGridData(myLoadType, organizationId, startDate, endDate) {
    //parent.$.messager.progress({ text: '数据加载中....' });
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "ProcessElectricityConsumption.aspx/GetData",
        data: '{organizationId: "' + organizationId + '", startDate: "' + startDate + '", endDate: "' + endDate + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            loadDataGrid("last", m_MsgData);
            //合并单元格
            //myMergeCell("gridMain_ReportTemplate", "ProductLineName");
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
    var startDate = $('#startDate').datebox('getValue');//开始时间
    var endDate = $('#endDate').datebox('getValue');//结束时间
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
        $('#gridMain_ReportTemplate').treegrid({
            columns: [[                    
                    { field: 'Name', title: '工序', width: 200},
                    { field: 'FormulaValue', title: '电量', width: 100,  align: "center" },
                    { field: 'DenominatorValue', title: '产量', width: 100, align: "center" },
                    { field: 'ElectricityConsumption', title: '电耗', width: 100, align: "center" }
            ]],
            fit:true,
            toolbar: "#toolbar_ReportTemplate",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            onClickRow: onClickRow,
            idField: 'id',
            treeField: 'Name',
            data: []
        })
    }
    else {
        $('#gridMain_ReportTemplate').treegrid("loadData", myData);
    }
}

function RefreshFun() {
    QueryReportFun();
}



//------------编辑单元格-------
var editIndex = undefined;
function endEditing() {
    if (editIndex == undefined) { return true }
    if (myDataGridObject.datagrid('validateRow', editIndex)) {
        var ed = myDataGridObject.datagrid('getEditor', { index: editIndex, field: 'FormulaValueB' });
        var formulaValueB = $(ed.target).val();
        myDataGridObject.datagrid('getRows')[editIndex]['FormulaValueB'] = formulaValueB;
        var ed = myDataGridObject.datagrid('getEditor', { index: editIndex, field: 'DenominatorValueB' });
        var denominatorValueB = $(ed.target).val();
        myDataGridObject.datagrid('getRows')[editIndex]['DenominatorValueB'] = denominatorValueB;
        var electricityConsumptionB = Number(denominatorValueB) == 0 ? 0 : (Number(formulaValueB) / Number(denominatorValueB)).toFixed(2);
        myDataGridObject.datagrid('updateRow', { index: editIndex, row: { ElectricityConsumptionB: electricityConsumptionB } });
        myDataGridObject.datagrid('endEdit', editIndex);
        myDataGridObject.datagrid('refreshRow', editIndex);
        doMergeCell("gridMain_ReportTemplate", "ProductLineName", merges);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}
//行单击事件
function onClickRow(index) {

    if (editIndex != index) {
        if (endEditing()) {
            myDataGridObject.datagrid('selectRow', index)
                    .datagrid('beginEdit', index);
            editIndex = index;
        } else {
            myDataGridObject.datagrid('selectRow', editIndex);
        }
    }
}
//撤销
function reject() {
    myDataGridObject.datagrid('rejectChanges');
    editIndex = undefined;
    doMergeCell("gridMain_ReportTemplate", "ProductLineName", merges);
}


function ExportFileFun() {
    //endEditing();           //关闭正在编辑
    //var m_DataGridData = myDataGridObject.datagrid('getData');
    //var m_DataGridDataJson = '{"rows":' + JSON.stringify(m_DataGridData['rows']) + '}';
    //$.ajax({
    //    type: "POST",
    //    url: "ProcessElectricityConsumption.aspx/InitDataGridData",
    //    data: "{myDataGridData:'" + m_DataGridDataJson + "'}",
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    async: false,//同步执行
    //    success: function (msg) {
    //        //PrintHtml(msg.d);
    //    }
    //});



    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = "Parameter1";
    var m_Parameter2 = "Parameter2";

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "ProcessElectricityConsumption.aspx");

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

function PrintFileFun() {
    endEditing();           //关闭正在编辑
    var m_DataGridData = myDataGridObject.datagrid('getData');
    var m_DataGridDataJson = '{"rows":' + JSON.stringify(m_DataGridData['rows']) + '}';
    $.ajax({
        type: "POST",
        url: "ProcessElectricityConsumption.aspx/PrintFile",
        data: "{myDataGridData:'" + m_DataGridDataJson + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            PrintHtml(msg.d);
        }
    });
}