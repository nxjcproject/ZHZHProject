var g_timer;

$(function () {
    InitDate();
    loadDataGrid("first");
});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    //nowDate.setDate(nowDate.getDate() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth()) + '-' + beforeDate.getDate() + " 00:00:00";
    $('#startDate').datetimebox('setValue', beforeString);
    $('#endDate').datetimebox('setValue', nowString);
}
function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
        realtimeAlarm();
        //g_timer = setTimeout("realtimeAlarm()", 300000);
}

function setTimer() {
    g_timer = setTimeout("realtimeAlarm()", 300000);
}
function setHistory() {
    clearTimeout(g_timer);
    $('#gridMain_ReportTemplate').datagrid("loadData", []);
    $(".queryDate").show();
}
function realtimeAlarm() {
    if (document.getElementsByName("alarmType")[0].checked == false) {
        clearTimeout(g_timer);
        return;
    }
    else {
        $(".queryDate").hide();
    }
    var organizationId = $('#organizationId').val();
    $.ajax({
        type: "POST",
        url: "EnergyConsumptionAlarm.aspx/GetRealTimeAlarm",
        data: '{organizationId: "' + organizationId + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,//同步执行
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            loadDataGrid("last", m_MsgData);
            setTimer();
        },
        error: setTimer()
    });
}
function loadDataGrid(type, myData) {
    if (type == "first") {
        $('#gridMain_ReportTemplate').datagrid({
            columns: [[
                    { field: 'AlarmDateTime', title: '报警开始时间', width: 150, align: "center" },
                    { field: 'ProductLineName', title: '产线', width: 100},
                    { field: 'Name', title: '工序', width: 100 },
                    { field: 'EnergyConsumptionType', title: '报警类型', width: 100 },
                    { field: 'StandardValue', title: '报警上限', width: 100, align: "center" },
                    { field: 'ActualValue', title: '报警实际值', width: 100, align: "center" }
            ]],
            toolbar: "#toolbar_ReportTemplate",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        })
    }
    else {
        $('#gridMain_ReportTemplate').datagrid("loadData", myData);
    }
}

function QueryReportFun() {
    editIndex = undefined;
    var organizationId = $('#organizationId').val();
    var startTime = $('#startDate').datetimebox('getValue');//开始时间
    var endTime = $('#endDate').datetimebox('getValue');//结束时间
    if (organizationId == "" || startDate == "" || endDate == "") {
        $.messager.alert('警告', '请选择生产线和时间');
        return;
    }
    if (startDate > endDate) {
        $.messager.alert('警告', '结束时间不能大于开始时间！');
        return;
    }
    $.ajax({
        type: "POST",
        url: "EnergyConsumptionAlarm.aspx/GetHistoryAlarm",
        data: '{organizationId: "' + organizationId + '", startTime: "' + startTime + '", endTime: "' + endTime + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            m_MsgData = jQuery.parseJSON(msg.d);
            loadDataGrid("last", m_MsgData);
        },
        error: handleError
    });
}

function handleError() {
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}