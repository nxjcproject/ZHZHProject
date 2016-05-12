var IntervalTimer = 300000;
$(function () {
    loadDataGrid("first", { "rows": [], "total": 0 });
    realtimeElectricityConsumption();
});
function loadDataGrid(type, myData) {
    if (type == "first") {
        $('#gridMain_RealtimeReport').datagrid({
            frozenColumns: [[
                    { field: 'OrganizationName', title: '所属产线', width: 150, align: "center"},
                    { field: 'MachineName', title: '设备名称', width: 200, align: "center" }
            ]],
            columns: [[
                    { title: '昨日电量', width: 240, align: "center", colspan: 3 },
                    { title: '今日电量', width: 240, align: "center", colspan: 3 },
                    { title: '昨日电耗', width: 240, align: "center", colspan: 3 },
                    { title: '今日电耗', width: 240, align: "center", colspan: 3 }
            ],
            [
                    { field: 'ElectricityQuantityYA', title: '甲班', width: 80, align: "center" },
                    { field: 'ElectricityQuantityYB', title: '乙班', width: 80, align: "center" },
                    { field: 'ElectricityQuantityYC', title: '丙班', width: 80, align: "center" },
                    { field: 'ElectricityQuantityTA', title: '甲班', width: 80, align: "center" },
                    { field: 'ElectricityQuantityTB', title: '乙班', width: 80, align: "center" },
                    { field: 'ElectricityQuantityTC', title: '丙班', width: 80, align: "center" },
                    { field: 'ElectricityConsumptionYA', title: '甲班', width: 80, align: "center" },
                    { field: 'ElectricityConsumptionYB', title: '乙班', width: 80, align: "center" },
                    { field: 'ElectricityConsumptionYC', title: '丙班', width: 80, align: "center" },
                    { field: 'ElectricityConsumptionTA', title: '甲班', width: 80, align: "center" },
                    { field: 'ElectricityConsumptionTB', title: '乙班', width: 80, align: "center" },
                    { field: 'ElectricityConsumptionTC', title: '丙班', width: 80, align: "center" },
            ]],
            rownumbers: true,
            singleSelect: true,
            striped: true,
            fit: true,
            border:true,
            data: []
        })
    }
    else {
        $('#gridMain_RealtimeReport').datagrid("loadData", myData);
    }
}

function realtimeElectricityConsumption() {
    $.ajax({
        type: "POST",
        url: "MainMachineElectricityConsumptionMonitor.aspx/GetElectricityConsumption",
        data: '',
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

function setTimer() {
    setTimeout("realtimeElectricityConsumption()", IntervalTimer);
}