/**
http://james.padolsey.com/javascript/sorting-elements-with-jquery/
* jQuery.fn.sortElements
* --------------
* @param Function comparator:
*   Exactly the same behaviour as [1,2,3].sort(comparator)
*   
* @param Function getSortable
*   A function that should return the element that is
*   to be sorted. The comparator will run on the
*   current collection, but you may want the actual
*   resulting sort to occur on a parent or another
*   associated element.
*   
*   E.g. $('td').sortElements(comparator, function(){
*      return this.parentNode; 
*   })
*   
*   The <td>'s parent (<tr>) will be sorted instead
*   of the <td> itself.
*/
jQuery.fn.sortElements = (function () {

    var sort = [].sort;

    return function (comparator, getSortable) {

        getSortable = getSortable || function () { return this; };

        var placements = this.map(function () {

            var sortElement = getSortable.call(this),
                parentNode = sortElement.parentNode,

                // Since the element itself will change position, we have
                // to have some way of storing its original position in
                // the DOM. The easiest way is to have a 'flag' node:
                nextSibling = parentNode.insertBefore(
                    document.createTextNode(''),
                    sortElement.nextSibling
                );

            return function () {

                if (parentNode === this) {
                    throw new Error(
                        "You can't sort elements if any one is a descendant of another."
                    );
                }

                // Insert before flag:
                parentNode.insertBefore(this, nextSibling);
                // Remove flag:
                parentNode.removeChild(nextSibling);

            };

        });

        return sort.call(this, comparator).each(function (i) {
            placements[i].call(getSortable.call(this));
        });

    };

})();


function rankSort(a, b) {
    return $(a).children('.urlVotes').first().html() > $(b).children('.urlVotes').first().html() ? 1 : -1;
}

function rankSortDesc(a, b) {
    return $(a).children('.urlVotes').first().html() < $(b).children('.urlVotes').first().html() ? 1 : -1;
}

function addTimeSort(a, b) {
    return $(a).data('add_time') > $(b).data('add_time') ? 1 : -1;
}

function addTimeSortDesc(a, b) {
    return $(a).data('add_time') < $(b).data('add_time') ? 1 : -1;
}

function watchTimeSort(a, b) {
    return $(a).data('watch_time') > $(b).data('watch_time') ? 1 : -1;
}

function watchTimeSortDesc(a, b) {
    return $(a).data('watch_time') < $(b).data('watch_time') ? 1 : -1;
}

function statusUpdate (message, delay, level) {
    if (delay == undefined)
        delay = 5;

    $('#info').removeClass('infoStatus').removeClass('ui-state-highlight').removeClass('ui-state-error');

    switch (level) {
        case "status":
            $('#info').addClass('infoStatus');
            break;
        case "error":
            $('#info').addClass('ui-state-highlight');
            break;
        case "critical":
            $('#info').addClass('ui-state-error');
            break;
    }

    $('#info').fadeIn(500).html(message).delay(delay*1000).fadeOut(1000);
}

function confirm(message, title, onConfirm, onCancel) {
    $("<div></div>").html(message).attr('id', 'confirm').dialog({
        resizable: false,
        height: 200,
        width: 500,
        modal: true,
        closeOnEscape: false,
        title: title,
        buttons: {
            "Yes": function () {
                if (undefined != onConfirm)
                    onConfirm();
                $(this).dialog("close").remove();
            },
            Cancel: function () {
                if (undefined != onCancel)
                    onCancel();
                $(this).dialog("close").remove();
            }
        }
    });
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}