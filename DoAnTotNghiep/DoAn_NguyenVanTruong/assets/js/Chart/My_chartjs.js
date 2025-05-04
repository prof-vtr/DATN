$(document).ready(function () {
        $.ajax({
            type: "POST",
            url: "Admin/Data_Chart",
            data: JSON.stringify({}),
            contentType: "application/json.charset=utf-8",
            dataType: "Json",
            success: function (json) {
                var values = json.Data_Chart;
                var Doanhthu = parseFloat(values[0]);

                

            }
               
            })
    })