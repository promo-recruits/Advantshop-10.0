﻿/* @ngInject */
function ReviewsCtrl($element, $compile, $scope, $http, $filter, $templateCache, $timeout, toaster, Upload, $translate) {

    var ctrl = this,
        listRoot,
        items = {},
        form,
        formScope;

    ctrl.visibleFormCancelButton = false;
    ctrl.reviewIdActive = 0;

    ctrl.addItemInStorage = function (id, element) {
        items[id] = element;
    };

    ctrl.addForm = function (scope, element) {
        form = element;
        formScope = scope;
    };

    ctrl.getForm = function () {
        return form;
    };

    ctrl.reply = function (parentId) {
        items[parentId].append(ctrl.getForm());
        ctrl.moveFormInside(parentId);
        ctrl.formVisible = true;
        ctrl.focusInput();

    };

    ctrl.moveFormInside = function (parentId) {
        ctrl.reviewIdActive = parentId;
        ctrl.visibleFormCancelButton = true;
    };

    ctrl.formReset = function () {
        formScope.reset();
    };

    ctrl.moveFormDefault = function () {
        $element.append(form);
    };

    ctrl.formInStart = function () {
        ctrl.moveFormDefault();

        ctrl.formReset();

        ctrl.reviewIdActive = 0;
        ctrl.visibleFormCancelButton = false;
    };

    ctrl.addReview = function (actionUrl, name, email, text, parentId, files, agreement, captchaCode, captchaSource) {
        $(document).trigger("add_response");

        return Upload.upload({
            url: actionUrl,
            data: {
                entityId: ctrl.entityId,
                entityType: ctrl.entityType,
                name: name,
                email: email,
                text: text,
                parentId: parentId,
                agreement: agreement,
                captchaCode: captchaCode,
                captchaSource: captchaSource
            },
            file: files // or list of files (files) for html5 only
        });
    };

    ctrl.submit = function (form, actionUrl) {

        //if (form.form.captchaCode != undefined) {
        ctrl.addReview(actionUrl, form.name, form.email, form.text, form.reviewId, form.images, form.agreement, form.captchaCode, form.captchaSource).then(function (response) {

            if (response.data.error) {
                toaster.pop('error', response.data.errors);
                return;
            }

            var newReview = response.data.review;

            if (ctrl.moderate == false) {
                ctrl.renderReviewItem(newReview.ParentId, newReview.ReviewId, newReview.Name, $filter('date')(Date.now(), 'dd MMMM yyyy'), newReview.Text, newReview.Photos,
                    newReview.Likes, newReview.Dislikes, newReview.RatioByLikes);
                if (ctrl.onAddComment != null) {
                    ctrl.onAddComment($scope);
                }
            } else {
                toaster.pop('info', $translate.instant('Js.Reviews.ThxForReviewTitle'), $translate.instant('Js.Reviews.ThxForReviewMsg'));
            }
        });
        //}

        if (ctrl.showFormAfterDo === true) {
            ctrl.formInStart();
        } else {
            ctrl.formReset();
            ctrl.reviewIdActive = 0;
            ctrl.visibleFormCancelButton = false;
            ctrl.formVisible = false;
        }

    };

    ctrl.renderReviewItem = function (parentId, reviewId, name, date, text, photos, likes, dislikes, ratioByLikes) {
        var parentContainer, list, needContainer, reviewNew, htmlItem, before;

        if (items[parentId] != null) {

            list = items[parentId].children('ul');

            if (list.length > 0) {
                parentContainer = list;
                needContainer = false;
            } else {
                parentContainer = items[parentId];
                needContainer = true;
            }

        } else {

            if (listRoot) {
                parentContainer = listRoot;
            }
            if (parentContainer == null) {
                var reviewslist = $element[0].querySelector('.reviews-list');
                if (reviewslist != null) {
                    parentContainer = angular.element(reviewslist);
                }
            }
            if (parentContainer == null) {
                parentContainer = angular.element($element[0].querySelector('.js-reviews-list-root'));
            }
            needContainer = true;
            before = true;
        }

        ctrl.getHtmlReviewItem(needContainer).then(function (htmlItem) {
            reviewNew = angular.element(htmlItem);

            if (before) {
                parentContainer.before(reviewNew);
            } else {
                parentContainer.append(reviewNew);
            }

            var scopeItem = $scope.$new();

            scopeItem.parentId = parentId;
            scopeItem.reviewId = reviewId;
            scopeItem.name = name;
            scopeItem.date = $translate.instant(date);
            scopeItem.text = text;
            scopeItem.photos = photos;
            scopeItem.likes = likes;
            scopeItem.dislikes = dislikes;
            scopeItem.ratioByLikes = ratioByLikes;

            $compile(reviewNew)(scopeItem);
        });
    };

    ctrl.getHtmlReviewItem = function (needContainer) {
        return $http.get('reviewItemTemplate.html', { cache: $templateCache }).then(function (response) {
            var result = response.data;

            if (needContainer === true) {
                result = '<ul class="reviews-list">' + result + '</ul>';
            }

            return result;
        });
    };

    ctrl.cancel = function (form) {
        if (ctrl.showFormAfterDo === true) {
            ctrl.formInStart();
        } else {
            ctrl.formReset();
            ctrl.reviewIdActive = 0;
            ctrl.visibleFormCancelButton = false;
            ctrl.formVisible = false;
        }
    };

    ctrl.deleteReviewFromDB = function (reviewId, actionUrl) {
        return $http.post(actionUrl, { reviewId: reviewId });
    };

    ctrl.delete = function (reviewId, actionUrl) {
        if (items[reviewId] != null) {

            ctrl.deleteReviewFromDB(reviewId, actionUrl).then(function (response) {
                items[reviewId].remove();

                if (ctrl.onDeleteComment != null) {
                    ctrl.onDeleteComment($scope);
                }
            });
        }
    };

    ctrl.focusInput = function () {
        formScope.setAutofocus();
    };
};

export default ReviewsCtrl;