$(document).ready(function () {
    let imgOnTop = document.getElementById("img-on-top");
    let imgUploadInput = document.getElementById("img-upload");
    let imgUploadIcon = document.getElementById("img-upload-icon");
    
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

    $('.select2').select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: function () {
            return $(this).data('placeholder');
        },
        allowClear: true
    });
    $('#modal-tags').select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: "Select or add tags",
        allowClear: true,
        tags: true,
        tokenSeparators: [','],
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
                    results: data.map(tag => ({ id: tag.tagId, text: tag.tagName }))
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
            };
        }
    });
    
    $('#modal-tags').on('select2:select', function (e) {
        var data = e.params.data;
        if (data.newTag) {
            $.ajax({
                type: 'POST',
                url: '/Tag/Create',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                data: { tagName: data.text },
                success: function (response) {
                    var newOption = new Option(response.tagName, response.tagId, false, true);
                    $('#modal-tags').find('[value="' + data.id + '"]').replaceWith(newOption).trigger('change');
                },
                error: function (xhr) {
                    var errorResponse = xhr.responseJSON;
                    if (errorResponse && errorResponse.error) {
                        alert(errorResponse.error);
                    } else {
                        alert('Failed to create new tag.');
                    }
                    var selectedTags = $('#modal-tags').val();
                    selectedTags.pop();
                    $('#modal-tags').val(selectedTags).trigger('change');
                }
            });
        }
    });
    
    $('#save-template-settings').on('click', function () {
        var selectedTopicId = $('select[asp-for="TopicId"]').val();
        var selectedTopicText = $('select[asp-for="TopicId"] option:selected').text();
        
        var selectedTags = $('#modal-tags').val();
        var selectedTagsText = $('#modal-tags option:selected').map(function() {
            return $(this).text();
        }).get();
        
        if (!selectedTopicId) {
            alert('Please select a topic.');
            return;
        }
        
        $('input[name="TopicId"]').remove();
        $('input[name="SelectedTagIds"]').remove();
        
        $('form#create-template-form').append(`<input type="hidden" name="TopicId" value="${selectedTopicId}" />`);
        if (selectedTags) {
            selectedTags.forEach(function(tagId) {
                $('form#create-template-form').append(`<input type="hidden" name="SelectedTagIds" value="${tagId}" />`);
            });
        }
        
        $('#selected-topic').text(selectedTopicText);
        $('#selected-tags').empty();
        if (selectedTagsText.length > 0) {
            selectedTagsText.forEach(function(tagName) {
                $('#selected-tags').append(`<span class="badge bg-primary me-1">${tagName}</span>`);
            });
        }
        $('#templateSettingsModal').modal('hide');
    });


    $('#tags').select2({
        placeholder: "Select or add tags",
        allowClear: true,
        tags: true,
        tokenSeparators: [','],
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
                    results: data.map(tag => ({ id: tag.tagId, text: tag.tagName }))
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
            };
        }
    });
    
    $('#tags').on('select2:select', function (e) {
        var data = e.params.data;
        if (data.newTag) {
            $.ajax({
                type: 'POST',
                url: '/Tag/Create',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                data: { tagName: data.text },
                success: function (response) {
                    var newOption = new Option(response.tagName, response.tagId, false, true);
                    $('#tags').find('[value="' + data.id + '"]').replaceWith(newOption).trigger('change');
                },
                error: function (xhr) {
                    var errorResponse = xhr.responseJSON;
                    if (errorResponse && errorResponse.error) {
                        alert(errorResponse.error);
                    } else {
                        alert('Failed to create new tag.');
                    }
                    var selectedTags = $('#tags').val();
                    selectedTags.pop();
                    $('#tags').val(selectedTags).trigger('change');
                }
            });
        }
    });
        
    
    $("#templateAccessibility").select2({
        minimumResultsForSearch: Infinity
    })
    
    function getSampleAnswerHtml(questionIndex, questionType) {
        switch (questionType) {
            case "SingleLineString":
                return `
                    <input type="text" class="form-control" placeholder="Enter a single line of text" maxlength="150" name="Questions[${questionIndex}].SampleAnswer" />
                `;
            case "MultiLineText":
                return `
                    <textarea class="form-control" placeholder="Enter multiple lines of text" rows="3" name="Questions[${questionIndex}].SampleAnswer"></textarea>
                `;
            case "PositiveInteger":
                return `
                    <input type="number" class="form-control" placeholder="Enter a positive number" min="0" name="Questions[${questionIndex}].SampleAnswer" />
                `;
            case "Checkbox":
                return `
                    <div class="form-check">
                        <input type="checkbox" class="form-check-input" name="Questions[${questionIndex}].SampleAnswer" />
                        <label class="form-check-label">Check this box</label>
                    </div>
                `;
            default:
                return `<p class="text-muted">Select a question type to see the sample answer.</p>`;
        }
    }
    
    $(document).on("change", ".question-type-select", function () {
        const questionIndex = $(this).data("question-index");
        const questionType = $(this).val();
        const sampleAnswerContainer = $(`#sample-answer-${questionIndex}`);
        const sampleAnswerHtml = getSampleAnswerHtml(questionIndex, questionType);
        sampleAnswerContainer.html(sampleAnswerHtml);
    });

    
    $("#questions-container").sortable();
    $('#add-question-btn').click(function () {
        var questionIndex = $('.question-form').length;
        var questionForm = `
            <div class="question-form card mb-3">
                <div class="card-body">
                    <h5 class="card-title">Question ${questionIndex + 1}</h5>
                    <div class="row">
                       <div class="form-group col-md-8">
                            <input placeholder="Question Text" type="text" name="Questions[${questionIndex}].QuestionText" class="form-control" required />
                            <span class="text-danger field-validation-valid" data-valmsg-for="Questions[${questionIndex}].QuestionText" data-valmsg-replace="true"></span>
                       </div>
                        <div class="form-group col-md-3">
                            <select name="Questions[${questionIndex}].Type" class="form-control question-type-select" data-question-index="${questionIndex}" required>
                                <option value="">Select Question Type</option>
                                <option value="SingleLineString">Single Line String</option>
                                <option value="MultiLineText">Multi Line Text</option>
                                <option value="PositiveInteger">Positive Integer</option>
                                <option value="Checkbox">Checkbox</option>
                            </select>
                            <span class="text-danger field-validation-valid" data-valmsg-for="Questions[${questionIndex}].Type" data-valmsg-replace="true"></span>
                        </div>
                        <div class="template-img-top col-md-1">
                            <label for="img-upload-${questionIndex}">
                                <i class="bi bi-file-earmark-image h2 img-upload-icon" data-question-index="${questionIndex}" style="cursor: pointer;"></i>
                            </label>
                            <input id="img-upload-${questionIndex}" type="file" accept="image/png, image/jpeg" 
                                   class="form-control-file img-upload-input" style="display: none;" data-question-index="${questionIndex}" />
                        </div>
                    </div>
                    <img src="" id="img-preview-${questionIndex}" class="img-fluid mt-2 d-none question-image-preview" alt="Preview" />
                    <div id="sample-answer-${questionIndex}" class="form-group mt-3">
                        <p class="text-muted">Select a question type to see the sample answer.</p>
                    </div>
                    <div class="form-group my-2">
                        <textarea placeholder="Question Description" name="Questions[${questionIndex}].Description" class="form-control"></textarea>
                        <span class="text-danger field-validation-valid" data-valmsg-for="Questions[${questionIndex}].Description" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group form-check">
                        <input type="checkbox" name="Questions[${questionIndex}].ShowInResults" class="form-check-input" />
                        <label class="form-check-label">Show in Results</label>
                    <button type="button" class="close remove-question btn btn-sm btn-outline-danger" aria-label="Close">
                        <i class="bi bi-trash"></i>
                    </button>
                    </div>
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
            fileInput.value = "";
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
    
    $(document).on('click', '.remove-question', function () {
        $(this).closest('.question-form').remove();
    });
});