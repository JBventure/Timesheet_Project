
function preview_timesheet_on_calendar(timesheet_id) {
    if (timesheet_id < 1)
        return;
    $("#id").val(timesheet_id);
    $("#loader").show();
    $.ajax({
        url: '/Timesheet/GetTimesheetItems/' + timesheet_id,
        method: "GET",
        success: function (data) {
            var events = [];
            for (let index = 0; index < data.length; index++) {
                const element = data[index];
                events.push({
                    title: element.itemTitle + ": " + element.duration + "hrs",
                    start: element.date,
                    //className: ,
                    backgroundColor: element.color != null ? element.color : '#378006'
                });
                $('#calendar').fullCalendar('destroy');
                $('#calendar').fullCalendar({
                    header: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'month'
                    },

                    events: events,
                    weekMode: "variable"
                });
                $('#calendar').fullCalendar('gotoDate', data[0].date);
            }
        }
    });
}

function clear_row(row) {
    $(row).parent().parent().find("input").each(function () {
        $(this).val("");
    });

    $(row).parent().parent().each(function () {
        if ($(this).has("eligible")) {
            $(this).removeClass("btn-warning");
        }
    });

    $(row).parent().parent().find("select").each(function () {
        this.selectedIndex = 0;
    });
    $(row).parent().parent().find("select")[1].disabled = true;
}

function delete_row(row, tb) {
    var rows = $("#" + tb + " tbody tr").length;
    if (rows > 1) {
        $(row).parent().parent().remove();
    } else {
        clear_row(row);
    }
}

function enable_activities(select) {
    var value = $(select).val();
    if (value == "") {
        $(select).closest('td').next().find('select')[0].selectedIndex = 0;
        $(select).closest('td').next().find('select')[0].disabled = true;
        //call on changes
        $(select).closest('td').next().find('select')[0].trigger("change");
        var controls = $(select).parent().parent().find(".eligible");
        for (var i = 0; i < controls.length; i++) {
            $(controls[i]).attr("disabled", "disabled");
            $(controls[i]).val("");
        }
    }
    else {
        var obj = $(select).closest('td').next().find('select')[0];
        //$(select).closest('td').next().find('select')[0].name = "";

        var control = $("[name='" + $(select).closest('td').next().find('select')[0].name + "']");
        control.empty();
        for (var i = 0; i < categories.length; i++) {
            if (categories[i].project_id == value) {
                control.append("<option value='" + categories[i].activity_id + "'>" + categories[i].name + "</option>");
                //$(select).closest('td').next().find('select')[0].append("<option value='" + categories[i].activity_id + "'>" + categories[i].name + "</option");
            }
        }

        $(select).closest('td').next().find('select')[0].disabled = false;

        if ($(select).val() == "") {
            var controls = $(select).parent().parent().find(".eligible");
            for (var i = 0; i < controls.length; i++) {
                $(controls[i]).attr("disabled", "disabled");
                $(controls[i]).val("");
            }
        }
        else {
            var controls = $(select).parent().parent().find(".eligible");
            for (var i = 0; i < controls.length; i++) {
                $(controls[i]).removeAttr("disabled");
            }
        }
    }
}

function enable_inputs(row) {
    if ($(row).val() == "") {
        var controls = $(row).parent().parent().find(".eligible");
        for (var i = 0; i < controls.length; i++) {
            $(controls[i]).attr("disabled", "disabled");
            $(controls[i]).val("");
        }
    }
    else {
        var controls = $(row).parent().parent().find(".eligible");
        for (var i = 0; i < controls.length; i++) {
            $(controls[i]).removeAttr("disabled");
        }
    }

}

function active_comment(id) {
    var ctrl = $(id).next();
    //active_comment_control = $(id).next()[0].id;
    var nxt = $(id).next()[0];

    active_comment_control = $(id).next()[0].id;

    $("#comment").val($("#" + active_comment_control).val());
    // $(id).addClass("btn-warning");
}

function add_comment() {
    var ctrl = $("#" + active_comment_control).prev();
    if ($("#comment").val().trim() != "") {
        $("#" + active_comment_control).prev().addClass("btn-warning");
    }
    else {
        $("#" + active_comment_control).prev().removeClass("btn-warning");
    }
    $("#" + active_comment_control).val($("#comment").val().trim());
    $('#myModal').modal('toggle');
}

function validate_entry() {
    var pattern = new RegExp("^023-[0-9]{0,7}$");

    $("input:text").change(function (e) {
        if (!pattern.test($(this).val())) {
            return false;
        }
    });
}

function getCategories() {
    $.ajax({
        url: '/TimeSheet/GetTimesheetActivityLnk',
        method: "GET",
        success: function (data) {
            categories = [];
            for (var i = 0; i < data.length; i++) {
                categories.push({ project_id: data[i].project_id, activity_id: data[i].activity_id, name: data[i].name });
            }
        }
    });
}

function add_row(tb) {
    var $tableBody = $('#' + tb).find("tbody"),
        $trLast = $tableBody.find("tr:last"),
        $trNew = $trLast.clone();

    var row_count = $trLast.index() + 1;
    $trNew.find('input,select').each(function () {
        var name = this.name;
        this.name = name.substring(0, name.length - 1) + (row_count + 1);
        var id = this.id;
        this.id = id.substring(0, name.length - 1) + (row_count + 1);
    });

    //reset the project selection
    $trNew.find('select').each(function () {
        this.selectedIndex = 0;
    });
    //disable activity dropdown
    //$trNew.find('select')[1].disabled = true;

    $trNew.find('button').each(function () {
        if ($(this).hasClass("eligible")) {
            $(this).removeClass("btn-warning");
        }
    });

    $trNew.find('input').each(function () {
        $(this).val("");
        $(this).attr("disabled", "disabled");
    });

    $trLast.after($trNew);
}
