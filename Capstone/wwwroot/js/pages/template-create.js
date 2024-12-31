$(document).ready(function () {
    let newTags = [];
    let imgOnTop = document.getElementById("img-on-top");
    let imgUploadInput = document.getElementById("img-upload");
    let imgUploadIcon = document.getElementById("img-upload-icon");

    function syncContent() {
        $('#hidden-template-title').val($('#template-title').text().trim());
        $('#hidden-template-description').val($('#template-description').text().trim());
    }

    $('#create-template-form').on('submit', function (e) {
        syncContent();
    });

    $('#template-title, #template-description').on('blur', function () {
        syncContent();
    });
    
    if (imgOnTop.getAttribute("src") === "") {
        $("#img-on-top").hide();
    }
    
    imgUploadInput.addEventListener("change", function (e) {
        if (e.target.files && e.target.files[0]) {
            imgOnTop.src = URL.createObjectURL(e.target.files[0]);
            $("#img-on-top").show();
            switchImgButton(false);
        }
    });
    
    imgUploadIcon.addEventListener("click", function (e) {
        e.preventDefault();

        if (imgUploadIcon.classList.contains("bi-file-earmark-minus")) {
            imgOnTop.src = "";
            imgUploadInput.value = ""; 
            $("#img-on-top").hide();
            switchImgButton(true);
        } else {
            imgUploadInput.click();
        }
    });
    
    function switchImgButton(isEmpty) {
        if (isEmpty) {
            imgUploadIcon.classList.remove("bi-file-earmark-minus");
            imgUploadIcon.classList.add("bi-file-earmark-image");
        } else {
            imgUploadIcon.classList.remove("bi-file-earmark-image");
            imgUploadIcon.classList.add("bi-file-earmark-minus");
        }
    }

    $("#modal-tags").select2({
        tags: true,
        tokenSeparators: [',', ' '],
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: "Select or add tags",
        allowClear: true,
        ajax: {
            url: '/Tag/Search',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.map(tag => ({ id: tag.id, text: tag.tagName }))
                };
            },
            cache: true
        },
        createTag: function (params) {
            var term = $.trim(params.term);
            if (term === '') {
                return null;
            }
            return {
                id: term,
                text: term,
                newTag: true
            }
        },
    })

    $('#modal-tags').on('select2:select', function (e) {
        const selectedTag = e.params.data;

        if (selectedTag.newTag) {
            if (!newTags.includes(selectedTag.text)) {
                newTags.push(selectedTag.text);
            }
        }
    });

    $('#modal-tags').on('select2:unselect', function (e) {
        const deselectedTag = e.params.data;

        if (deselectedTag.newTag) {
            const index = newTags.indexOf(deselectedTag.text);
            if (index > -1) {
                newTags.splice(index, 1);
            }
        }
    });
    
    $("#templateUsers").select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: "Search and select users",
        allowClear: true,
        ajax: {
            url: '/User/Search',
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    q: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.map(user => ({
                        id: user.id,
                        text: `${user.email}`
                    }))
                };
            },
            cache: true
        }
    });
    
    function getSampleAnswerHtml(questionIndex, questionType) {
        switch (questionType) {
            case "SingleLineString":
                return `
                    <input disabled type="text" class="form-control" placeholder="Enter a single line of text" maxlength="150" name="Questions[${questionIndex}].SampleAnswer" />
                `;
            case "MultiLineText":
                return `
                    <textarea disabled class="form-control" placeholder="Enter multiple lines of text" rows="3" name="Questions[${questionIndex}].SampleAnswer"></textarea>
                `;
            case "PositiveInteger":
                return `
                    <input disabled type="number" class="form-control" placeholder="Enter a positive number" min="0" name="Questions[${questionIndex}].SampleAnswer" />
                `;
            case "Checkbox":
                return `
                <div class="options-container" id="options-container-${questionIndex}">
                </div>
                <div class="form-check my-2">
                    <input disabled class="form-check-input" type="checkbox" value="">
                     <button type="button" class="btn btn-sm btn-primary add-option" data-question-index="${questionIndex}">Add Option</button>
                </div>
`;
            default:
                return `<p class="text-muted">Select a question type to see the sample answer.</p>`;
        }
    }

    $(document).on("click", ".add-option", function () {
        const questionIndex = $(this).data("question-index");
        const optionsContainer = $(`#options-container-${questionIndex}`);
        const optionIndex = optionsContainer.find(".option-group").length;

        const optionHtml = `
        <div class="option-group mb-2">
            <div class="form-check">
                <input class="form-check-input border border-primary" type="checkbox" value="">
                <input type="text" class="form-check-label border-0 border-bottom" placeholder="Option ${optionIndex + 1}" name="Questions[${questionIndex}].QuestionOptions[${optionIndex}].OptionText" required>
                <button type="button" class="btn btn-sm text-danger remove-option p-0 border-0">
                    <i class="bi bi-x-circle h5"></i>
                </button>
            </div>
        </div>
    `;
        optionsContainer.append(optionHtml);
    });

    $(document).on("click", ".remove-option", function () {
        $(this).closest(".option-group").remove();
    });
    
    $(document).on("change", ".question-type-select", function () {
        const questionIndex = $(this).data("question-index");
        const questionType = $(this).val();
        const sampleAnswerContainer = $(`#sample-answer-${questionIndex}`);
        const sampleAnswerHtml = getSampleAnswerHtml(questionIndex, questionType);
        sampleAnswerContainer.html(sampleAnswerHtml);
    });

    
    $("#questions-container").sortable({
        axis: "y",
        update: function (event, ui) {
            $(".question-form").each(function (index) {
                $(this).find('input[name*="Order"]').val(index);
                $(this).find('.card-title').text(`Question ${index + 1}`);
            });
        }
        });
    
    $('#add-question-btn').click(function () {
        var questionIndex = $('.question-form').length;
        var questionForm = `
    <div class="question-form card mb-3" id=questionIndex>
        <div class="card-body">
            <h5 class="card-title">Question ${questionIndex + 1}</h5>
            <div class="row">
                <div class="form-group col-md-8">
                    <input placeholder="Question Text" type="text" name="Questions[${questionIndex}].QuestionText" class="form-control" required />
                </div>
                <div class="form-group col-md-3">
                    <select name="Questions[${questionIndex}].Type" class="form-control question-type-select" data-question-index="${questionIndex}" required>
                        <option value="">Select Question Type</option>
                        <option value="SingleLineString">Single Line String</option>
                        <option value="MultiLineText">Multi Line Text</option>
                        <option value="PositiveInteger">Positive Integer</option>
                        <option value="Checkbox">Checkbox</option>
                    </select>
                </div>
                <div class="form-group template-img-top col-md-1">
                    <label for="img-upload-${questionIndex}">
                        <i class="bi bi-file-earmark-image h2 img-upload-icon" data-question-index="${questionIndex}" style="cursor: pointer;"></i>
                    </label>
                    <input id="img-upload-${questionIndex}" type="file" accept="image/png, image/jpeg" name="Questions[${questionIndex}].QuestionImage"
                           class="form-control-file img-upload-input" style="display: none;" data-question-index="${questionIndex}" />
                </div>
            </div>
            <img src="" id="img-preview-${questionIndex}" class="img-fluid mt-2 d-none question-image-preview" alt="Preview" />
            <div id="sample-answer-${questionIndex}" class="form-group my-3">
                <p class="text-muted">Select a question type to see the sample answer.</p>
            </div>
            <div class="form-group">
                <textarea placeholder="Question Description" name="Questions[${questionIndex}].Description" class="form-control"></textarea>
            </div>
            <div class="form-group form-check">
                <input type="checkbox" name="Questions[${questionIndex}].ShowInResults" class="form-check-input border border-primary" />
                <label class="form-check-label">Show in Results</label>
            </div>
            <button type="button" class="close remove-question btn btn-sm btn-outline-danger">
                <i class="bi bi-trash"></i>
            </button>
            <input type="hidden" name="Questions[${questionIndex}].Order" value="${questionIndex}" />
        </div>
    </div>
`;
        $('#questions-container').append(questionForm);
    });

    $(document).on("change", ".img-upload-input", function (e) {
        const questionIndex = $(this).data("question-index");
        const fileInput = e.target;
        const previewImg = $(`#img-preview-${questionIndex}`);
        const uploadIcon = $(`.img-upload-icon[data-question-index="${questionIndex}"]`);

        if (fileInput.files && fileInput.files[0]) {
            previewImg.attr("src", URL.createObjectURL(fileInput.files[0]));
            previewImg.removeClass("d-none");
            switchImgIconButton(uploadIcon, false);
        }
    });

    $(document).on("click", ".img-upload-icon", function (e) {
        e.preventDefault();
        const questionIndex = $(this).data("question-index");
        const fileInput = $(`#img-upload-${questionIndex}`)[0];
        const previewImg = $(`#img-preview-${questionIndex}`);
        const uploadIcon = $(this);

        if (uploadIcon.hasClass("bi-file-earmark-minus")) {
            previewImg.attr("src", "").addClass("d-none");
            switchImgIconButton(uploadIcon, true);
        } else {
            fileInput.click();
        }
    });
    function switchImgIconButton(icon, isEmpty) {
        if (isEmpty) {
            icon.removeClass("bi-file-earmark-minus").addClass("bi-file-earmark-image");
        } else {
            icon.removeClass("bi-file-earmark-image").addClass("bi-file-earmark-minus");
        }
    }
    
    $('#templateAccessibility').on('change', function () {
        var selectedAccess = $(this).val();
        if (selectedAccess === 'false') {
            $('#templateUserGroup').show();
        } else {
            $('#templateUserGroup').hide();
        }
    });

    $('#save-template-settings').on('click', function () {
        var selectedTopicId = $("#TemplateTopic").val();
        var selectedTopicText = $("#TemplateTopic option:selected").text();
        var selectedTags = $('#modal-tags').val();
        var selectedTagsText = $('#modal-tags option:selected').map(function() {
            return $(this).text();
        }).get();
        var selectedAccess = $('#templateAccessibility').val();
        var selectedUsers = $('#templateUsers').val();
        var selectedUsersText = $('#templateUsers option:selected').map(function() {
            return $(this).text();
        }).get();
        

        $('input[name="TopicId"]').remove();
        $('input[name="SelectedTagIds"]').remove();
        $('input[name="IsPublic"]').remove();
        $('input[name="TemplateUserIds"]').remove();

        $('form#create-template-form').append(`<input type="hidden" name="TopicId" value="${selectedTopicId}" />`);
        if (selectedTags) {
            selectedTags.forEach(function(tagId) {
                $('form#create-template-form').append(`<input type="hidden" name="SelectedTagIds" value="${tagId}" />`);
            });
        }

        $('form#create-template-form').append(`<input type="hidden" name="IsPublic" value="${selectedAccess}" />`);
        if (selectedAccess === 'false' && selectedUsers) {
            selectedUsers.forEach(function(userId) {
                $('form#create-template-form').append(`<input type="hidden" name="TemplateUserIds" value="${userId}" />`);
            });
        }

        if (newTags.length > 0) {
            $.ajax({
                url: '/Tag/Create',
                type: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                contentType: 'application/json',
                data: JSON.stringify(newTags),
                success: function (savedTags) {
                    newTags = [];
                },
                error: function (err) {
                    alert('Failed to save new tags.');
                }
            });
        }
        $('#templateSettingsModal').modal('hide');
    });

    
    $(document).on('click', '.remove-question', function () {
        $(this).closest('.question-form').remove();
        $(".question-form").each(function (index) {
            $(this).find('input[name*="Order"]').val(index);
            $(this).find('.card-title').text(`Question ${index + 1}`);
        });
    });

    $('#create-template-form').on('submit', function (e) {
        const formData = new FormData(this);
        for (let [key, value] of formData.entries()) {
            console.log(key, value);
        }
    });
});