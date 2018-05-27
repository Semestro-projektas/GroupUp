var count = 1;


function addItem() {
    var itemVal = $('.form-control-skills').val();
    var item = '<span class="label label-default expertise">' + itemVal + '<i id="close" class="fa fa-times pr-1 ml-1"> </i></span>\n';
    var isError = $('.form-control-skills').hasClass('hidden');

    if (!(itemVal === '')) {
        $('.err').addClass('hidden');
        $('.todo-list-' + count.toString()).append(item);
    }

    $('.refresh').removeClass('hidden');

    $('.no-items').addClass('hidden');

    $('.form-control-skills').val('').attr('placeholder', 'baking pies...');
    setTimeout(function () {
        $('.todo-list li').removeClass('animated');
    }, 500);

}

function refresh() {
    $('.todo-list li').each(function (i) {
        $(this).delay(70 * i).queue(function () {
            $(this).addClass('animated bounceOutLeft');
            $(this).dequeue();
        });
    });
}

$(function () {

    var err = $('.err'),
        formControl = $('.form-control-skills'),
        isError = formControl.hasClass('hidden');

    if (!isError) {
        formControl.blur(function () {
            err.addClass('hidden');
        });
    }

    $('.add-btn').on('click', function () {
        addItem();
        formControl.focus();
    });


    $('.todo-list-1').on('click', '.label', function () {
        var box = $(this);
        box.removeClass('animated');

        setTimeout(function () {
            box.remove();
        }, 10);
    });

    $('.form-control-skills').keypress(function (e) {
        if (e.which === 13) {
            addItem();
        }
    });

    //$('.todo-list').disableSelection();

});