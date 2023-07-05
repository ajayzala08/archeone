$(document).ready(function () {
    // Get the ID of the initial input field
    var inputId = $('.inputTags').attr('id');
    var placeHolder = $('.inputTags').attr('placeholder');
    var className = $('.inputTags').attr('class');

    // Create the new structure
    var tagsInput = $('<div class="tags-input form-group" style="border: 1px solid #ced4da;border-radius: 0.25rem;box-shadow: inset 0 0 0 transparent;width: 100 %;" onclick="handleClickEvent(event)">');
    var row = $('<div class="row">');
    var tagsList = $('<ul id="tags" class="ml-2" style="padding: 0;margin: 0;"></ul>');
    var inputTag = $('<input type="text" id="input-tag" class="ml-2 ' + className + '" style="border: none;outline: none;padding: 0.375rem;font-weight: 400;color: #495057;background-color: #fff;background-clip: padding-box;" onkeydown="handleKeyDown(event)" onblur="handleKeyDown(event)"/>');

    // Set the ID of the new input field
    inputTag.attr('id', inputId);

    // Set the PlaceHolder of the new input field
    inputTag.attr('placeHolder', placeHolder);

    // Append the elements to the new structure
    row.append(tagsList, inputTag);
    tagsInput.append(row);

    // Replace the initial input field with the new structure
    $('.inputTags').replaceWith(tagsInput);
});

// Extend the jQuery functionality
$.fn.extend({
    value: function (value) {
        if (value != null && value != "") {
            $("#tags").empty();
            if (value.toString().includes(",")) {
                var valueArray = value.split(", ");
                valueArray.forEach(function (element) {
                    $('#tags').append(createNewTag(element));
                });
            } else {
                $('#tags').append(createNewTag(value));
            }
        }

        var tagTexts = $('#tags li').map(function () {
            return $(this).clone().children('button').remove().end().text().trim();
        }).get();

        return tagTexts;
    }
});
function handleKeyDown(event) {
    if (event.key === 'Enter' || event.key === "," || event.type === "blur") {

        // Prevent the default action of the keypress
        // event (submitting the form)
        event.preventDefault();

        // Get the trimmed value of the input element
        const tagContent = $(".inputTags").val().trim();

        var tagTexts = $('#tags li').map(function () {
            return $(this).clone().children('button').remove().end().text().trim();
        }).get();

        var isDuplicated = tagTexts.some(function (text) {
            return text == tagContent;
        });

        // If the trimmed value is not an empty string
        if (tagContent !== '' && !isDuplicated) {
            // Append the tag to the tags list
            $('#tags').append(createNewTag(tagContent));

            // Clear the input element's value
            $(".inputTags").val('');

        }
    }
}

function handleClickEvent(event) {
    if (event.target.classList.contains('delete-button')) {

        // Remove the parent element (the tag)
        event.target.parentNode.remove();
    }
}

function createNewTag(tagContent) {
    // Create a new list item element for the tag
    var tag = $('<li class="btn btn-primary btn-sm m-1 mr-sm-0">');

    // Set the text content of the tag to the trimmed value
    tag.text(tagContent);

    // Add a delete button to the tag
    tag.append('<button class="delete-button text-white" style="background-color: transparent;border: none;color: #999;cursor: pointer;margin - left: 5px;">&times;</button>');

    return tag;
}
