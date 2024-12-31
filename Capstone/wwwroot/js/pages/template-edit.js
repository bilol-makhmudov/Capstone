$(document).ready(function () {
    let imgOnTop = document.getElementById("img-on-top");
    let imgUploadInput = document.getElementById("img-upload");
    let imgUploadIcon = document.getElementById("img-upload-icon");

    
    if (model.ImageUrl) {
        imgOnTop.src = model.ImageUrl;
        $("#img-on-top").show();
        switchImgButton(false);
    } else {
        $("#img-on-top").hide();
    }
    
    if (model.Questions && model.Questions.length > 0) {
        
        model.Questions.forEach((q, index) => {
            addExistingQuestion(q, index);
            });
    }
    

    function addExistingQuestion(q, questionIndex) {
        const questionForm = `
    <div class="question-form card mb-3">
        <div class="card-body">
            <h5 class="card-title">Question ${questionIndex + 1}</h5>
            <div class="row">
                 <div class="form-group col-md-8">
                     <input placeholder="Question Text" type="text" name="Questions[${questionIndex}].QuestionText"
                           class="form-control" value="${q.QuestionText ?? ""}" required />
                </div>
                <div class="form-group col-md-3">
                    <select name="Questions[${questionIndex}].Type" class="form-control question-type-select"
                            data-question-index="${questionIndex}" required>
                        <option value="">Select Question Type</option>
                        <option value="SingleLineString" ${q.Type === 1 ? "selected" : ""}>Single Line String</option>
                        <option value="MultiLineText" ${q.Type === 2 ? "selected" : ""}>Multi Line Text</option>
                        <option value="PositiveInteger" ${q.Type === 3 ? "selected" : ""}>Positive Integer</option>
                        <option value="Checkbox" ${q.Type === 4 ? "selected" : ""}>Checkbox</option>
                    </select>
                </div>
                <div class="form-group template-img-top col-md-1">
                    <label for="img-upload-${questionIndex}">
                        <i class="bi bi-file-earmark-image h2 img-upload-icon" data-question-index="${questionIndex}"
                           style="cursor: pointer;"></i>
                    </label>
                    <input id="img-upload-${questionIndex}" type="file" accept="image/png, image/jpeg"
                           name="Questions[${questionIndex}].QuestionImage"
                           class="form-control-file img-upload-input d-none"
                           data-question-index="${questionIndex}" />
                </div>
            </div>
            <img src="${q.QuestionImageUrl ?? ''}" id="img-preview-${questionIndex}"
                 class="img-fluid mt-2 ${q.QuestionImageUrl ? '' : 'd-none'} question-image-preview" alt="Preview" />
            
            <div id="sample-answer-${questionIndex}" class="form-group my-3">
            </div>

            <div class="form-group">
                <textarea placeholder="Question Description" name="Questions[${questionIndex}].Description"
                          class="form-control">${q.Description ?? ""}</textarea>
            </div>
            <div class="form-group form-check">
                <input type="checkbox" name="Questions[${questionIndex}].ShowInResults" class="form-check-input border border-primary"
                       ${q.ShowInResults ? "checked" : ""} />
                <label class="form-check-label">Show in Results</label>
            </div>
            <button type="button" class="close remove-question btn btn-sm btn-outline-danger">
                <i class="bi bi-trash"></i>
            </button>
            <input type="hidden" name="Questions[${questionIndex}].Id" value="${q.Id || ''}" />
            <input type="hidden" name="Questions[${questionIndex}].Order" value="${q.Order || questionIndex}" />
            <input type="hidden" name="Questions[${questionIndex}].RowVersionBase64" value="${q.RowVersionBase64 || ''}" /> 
        </div>
    </div>
    `;
        $("#questions-container").append(questionForm);
        
        const sampleAnswerHtml = getSampleAnswerHtml(questionIndex, q.Type);
        $(`#sample-answer-${questionIndex}`).html(sampleAnswerHtml);
        
        if (q.Type === 4 && q.QuestionOptions && q.QuestionOptions.length > 0) {
            q.QuestionOptions.forEach((opt, optIndex) => {
                const optionHtml = `
            <div class="option-group mb-2">
                <div class="form-check">
                    <input class="form-check-input border border-primary" type="checkbox" value="">
                    <input type="text"
                           class="form-check-label border-0 border-bottom"
                           placeholder="Option ${optIndex + 1}"
                           name="Questions[${questionIndex}].QuestionOptions[${optIndex}].OptionText"
                           value="${opt.OptionText ?? ""}"
                           required>
                    <button type="button" class="btn btn-sm text-danger remove-option p-0 border-0">
                        <i class="bi bi-x-circle h5"></i>
                    </button>
                </div>
            </div>
            `;
                $(`#options-container-${questionIndex}`).append(optionHtml);
            });
        }
        if (q.QuestionImageUrl) {
            $(`.img-upload-icon[data-question-index="${questionIndex}"]`)
                .removeClass("bi-file-earmark-image")
                .addClass("bi-file-earmark-minus");
        } else {
            $(`.img-upload-icon[data-question-index="${questionIndex}"]`)
                .removeClass("bi-file-earmark-minus")
                .addClass("bi-file-earmark-image");
        }
    }

    function syncContent() {
        $('#hidden-template-title').val($('#template-title').text().trim());
        $('#hidden-template-description').val($('#template-description').text().trim());
    }

    $('#edit-template-form').on('submit', function (e) {
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
    
    function getSampleAnswerHtml(questionIndex, questionType) {
        if(questionType === "SingleLineString" || questionType === 1){
            return `
                    <input disabled type="text" class="form-control" placeholder="Enter a single line of text" maxlength="150" name="Questions[${questionIndex}].SampleAnswer" />
                `;
        }
        else if(questionType === "MultiLineText" || questionType === 2){
            return `
                    <textarea disabled class="form-control" placeholder="Enter multiple lines of text" rows="3" name="Questions[${questionIndex}].SampleAnswer"></textarea>
                `;
        }
        else if(questionType === "PositiveInteger" || questionType === 3){
            return `
                    <input disabled type="number" class="form-control" placeholder="Enter a positive number" min="0" name="Questions[${questionIndex}].SampleAnswer" />
                `;
        }
        else if(questionType === "Checkbox" || questionType === 4){
            return `
                <div class="options-container" id="options-container-${questionIndex}">
                </div>
                <div class="form-check my-2">
                    <input disabled class="form-check-input" type="checkbox" value="">
                     <button type="button" class="btn btn-sm btn-primary add-option" data-question-index="${questionIndex}">Add Option</button>
                </div>`;
        }
        else{
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

    $(document).on('click', '.remove-question', function () {
        $(this).closest('.question-form').remove();
        $(".question-form").each(function (index) {
            $(this).find('input[name*="Order"]').val(index);
            $(this).find('.card-title').text(`Question ${index + 1}`);
        });
    });

    $('#edit-template-form').on('submit', function (e) {
        const formData = new FormData(this);
        for (let [key, value] of formData.entries()) {
            console.log(key, value);
        }
    });
});