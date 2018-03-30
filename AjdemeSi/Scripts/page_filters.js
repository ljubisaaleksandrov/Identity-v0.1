var Contacts_Controller_Name = "Contacts";
var userRolesSelector;

// #region constants
var scA = 'sorting_asc';
var scD = 'sorting_desc';
var scN = 'sorting';
var scFC = '.filters-sort';
var scFD = 'filter-name';

var cntPC = '.filter-count-on-page';
var sQueryC = '.filter-search-query';

var contNID = '#ControlerName';

var dpFDID = '#DateFrom';
var dpTDID = '#DateTo';
var dpDD = 'current-time'
// #endregion constants

$(document).ready(function () {
    $('.datepicker').datepicker({
        autoClose: true,
        format: 'mm/dd/yyyy',
        date: $(this).data(dpDD),
        startDate: $(this).data(dpDD)
    });

    $(dpTDID).datepicker({
        autoClose: true,
        format: 'mm/dd/yyyy',
        date: $(this).data(dpDD),
        endDate: $(this).data(dpDD)
    });

    userRolesSelector = new SlimSelect({
        select: '.multiple-select-box',
        placeholder: $('.multiple-select-box').attr('placeholder'),
        onChange: (info) => {
            fnSubmitForm($('.multiple-select-box').parents('form'), 1, true);
        }
    })

    new Morris.Line({
        // ID of the element in which to draw the chart.
        element: 'myfirstchart',
        // Chart data records -- each entry in this array corresponds to a point on
        // the chart.
        data: [
            { year: '2008', value: 20 },
            { year: '2009', value: 10 },
            { year: '2010', value: 5 },
            { year: '2011', value: 5 },
            { year: '2012', value: 20 }
        ],
        // The name of the data record attribute that contains x-values.
        xkey: 'year',
        // A list of names of data record attributes that contain y-values.
        ykeys: ['value'],
        // Labels for the ykeys -- will be displayed when you hover over the
        // chart.
        labels: ['Value']
    });
});

var fnGetSearchQuery = function (currentForm) {
    if ($(currentForm).find(sQueryC).length) {
        return $(currentForm).find(sQueryC).val();
    }
    else { return null; }
}

var fnGetCountOnPage = function (currentForm) {
    if ($(currentForm).find(cntPC).length) {
        return parseInt($(currentForm).find(cntPC).find(":selected").val());
    }
    else { return null; }
}

var fnGetControllerName = function (currentForm) {
    if ($(currentForm).find(contNID).length) {
        return $(currentForm).find(contNID).val();
    }
    else { return null; }
}

var fnGetDateFrom = function (currentForm) {
    if ($(currentForm).find(dpFDID).length) {
        return $(currentForm).find(dpFDID).datepicker("getDate");
    }
    else { return null; }
}

var fnGetDateTo = function (currentForm) {
    if ($(currentForm).find(dpTDID).length) {
        return $(currentForm).find(dpTDID).datepicker("getDate");
    }
    else { return null; }
}

var fnResetDateFrom = function (currentForm) {
    if ($(currentForm).find(dpFDID).length) {
        $(currentForm).find(dpFDID).data("is-initialized", false);
        var value = $(currentForm).find('#postbackDateFrom').val();
        $(currentForm).find(dpFDID).datepicker("setDate", value);
    }
}

var fnResetDateTo = function (currentForm) {
    if ($(currentForm).find(dpTDID).length) {
        $(currentForm).find(dpTDID).data("is-initialized", false);
        var value = $(currentForm).find('#postbackDateTo').val();
        $(currentForm).find(dpTDID).datepicker("setDate", value);
    }
}

var fnGetSortOrder = function (currentForm) {
    var sort = "";
    if ($(currentForm).find(scFC).length) {
        $.each($(currentForm).find(scFC), function (index, element) {
            if ($(element).hasClass(scA)) {
                sort = $(element).data(scFD) + "_asc";
            }
            else if ($(element).hasClass(scD)) {
                sort = $(element).data(scFD) + "_desc";
            }
        });
    }
    return sort;
}

var fnSetSortOrder = function (currentForm, sortOrder) {
    var arguments = sortOrder.split('_');
    var isAscending = scA.indexOf(arguments[1]) >= 0;
    var element = $(currentForm).find(scFC + '*[data-' + scFD + '="' + arguments[0] + '"]');

    $(element).parent().find('.filters-sort').removeClass(scA).removeClass(scD).addClass(scN);
    $(element).removeClass(scN).addClass(isAscending ? scA : scD);
};

var fnGetUserRoles = function (selector) {
    return selector.selected();
};

var fnSubmitForm = function (form, pageNumber = 1, resetDate = false) {
    var controllerName = fnGetControllerName(form);
    var url = controllerName + '/ApplyFilters';
    var token = $(form).children('input[name="__RequestVerificationToken"]').val();

    var countOnPage = fnGetCountOnPage(form);
    var searchQuery = fnGetSearchQuery(form);
    var sortOrder = fnGetSortOrder(form);
    var dateFrom = resetDate ? "" : fnGetDateFrom(form);
    var dateTo = resetDate ? "" : fnGetDateTo(form);
    var userRoles = fnGetUserRoles(userRolesSelector);

    $.ajax({
        type: 'post',
        url: url,
        data: {
            __RequestVerificationToken: token,
            pageSize: countOnPage,
            searchString: searchQuery,
            sortOrder: sortOrder,
            pageNumber: pageNumber,
            dateFrom: dateFrom,
            dateTo: dateTo,
            selectedUserRoles: userRoles
        },
        success: function (data) {
            $(form).find('.entries-list').html(data);
            fnSetSortOrder(form, sortOrder);
            fnResetDateFrom(form);
            fnResetDateTo(form);
        },
        error: function (err) {
            console.log("AJAX error in request: " + JSON.stringify(err, null, 2));
        }
    });
}

$(document).on('click tap', scFC, function (event) {
    var isAscending = $(this).hasClass(scA) ? false : true;
    var filterName = $(this).data('filter-name');

    $(this).parent().find('.filters-sort').removeClass(scA).removeClass(scD).addClass(scN);

    $(this).removeClass(scN).addClass(isAscending ? scA : scD);

    fnSubmitForm($(this).parents('form'));
}); 

$(document).on('change', '.filter-count-on-page', function (event) {
    fnSubmitForm($(this).parents('form'));
});

$(document).on('keyup', '.filter-search-query', function (event) {
    fnSubmitForm($(this).parents('form'), 1, true);
});

$(document).on('changeDate', dpFDID, function (event) {
    if ($(this).data("is-initialized")) {
        $(this).datepicker('hide');
        fnSubmitForm($(this).parents('form'));
    }
    else {
        $(this).data("is-initialized", true);
    }
});

$(document).on('changeDate', dpTDID, function (event) {
    if ($(this).data("is-initialized")) {
        $(this).datepicker('hide');
        fnSubmitForm($(this).parents('form'));
    }
    else {
        $(this).data("is-initialized", true);
    }
});

$(document).on('click tap', '.pagination li', function (event) {
    event.preventDefault();
    event.stopPropagation();
    fnSubmitForm($(this).parents('form'), parseInt($(this).children('a').attr('href').split('=')[1]));
});

$(document).on('change', '.multiple-select-box', function (event) {
    console.log($(userRolesSelector.selected()));
});