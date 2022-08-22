﻿; (function (ng) {
    'use strict';

    var ReviewsCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, SweetAlert, $q, $uibModal, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: '_noopColumnName',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.Author'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    }
                },
                {
                    name: '_noopColumnEmail',
                    visible: false,
                    filter: {
                        placeholder: 'Email',
                        type: uiGridConstants.filter.INPUT,
                        name: 'Email',
                    }
                },
                {
                    name: '_noopColumnText',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.ReviewText'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Text',
                    }
                },
                {
                    name: '_noopColumnArtNo',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.VendorCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ArtNo',
                    }
                },
                {
                    name: 'PhotoName',
                    displayName: $translate.instant('Admin.Js.Reviews.Img'),
                    width: 80,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="ui-grid-custom-flex-center ui-grid-custom-link-for-img">' +
                            '<img ng-src="{{row.entity.PhotoSrc}}" alt="" />' + 
                        '</div></div>',
                    enableSorting: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.Image'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'HasPhoto',
                        selectOptions: [{ label: $translate.instant('Admin.Js.Reviews.WithPhoto'), value: true }, { label: $translate.instant('Admin.Js.Reviews.WithoutPhoto'), value: false }]
                    }
                },
                {
                    name: 'Name',
                    width: 270,
                    displayName: $translate.instant('Admin.Js.Reviews.Author'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                            '{{row.entity.Name}}<br/> ' +
                            '{{row.entity.Email}} ' +
                        '</div>',
                },
                {
                    name: 'Text',
                    displayName: $translate.instant('Admin.Js.Reviews.ReviewText'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +

                    //        '<ui-modal-trigger size="middle" data-controller="\'ModalAddEditReviewCtrl\'" controller-as="ctrl" ' +
                    //                        'template-url="../areas/admin/content/src/reviews/modal/addEditReview/addEditReview.html" ' +
                    //                        'data-resolve="{\'reviewId\': row.entity.ReviewId}" ' +
                    //                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                    //            '<a href="">{{ grid.appScope.$ctrl.gridExtendCtrl.truncate(row.entity.Text) }}</a>' +
                    //        '</ui-modal-trigger>' +
                            '{{ grid.appScope.$ctrl.gridExtendCtrl.truncate(row.entity.Text) }}' + 
                        '</div>'
                },
                {
                    name: 'ProductName',
                    displayName: $translate.instant('Admin.Js.Reviews.Name'),
                    enableSorting: false,
                    width: 160,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                            '<a ng-if="row.entity.ProductId != null" href="product/edit/{{row.entity.ProductId}}">{{row.entity.ProductName}}</a> ' +
                            '<span ng-if="row.entity.ProductId == null">{{row.entity.ArtNo}}, {{row.entity.ProductName}}</span>' +
                        '</div>'
                },
                {
                    name: 'AddDateFormatted',
                    displayName: $translate.instant('Admin.Js.Reviews.Added'),
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.DateAndTime'),
                        type: 'datetime',
                        term: {
                            from: new Date((new Date()).setMonth((new Date()).getMonth() - 1)),
                            to: new Date()
                        },
                        datetimeOptions: {
                            from: {
                                name: 'DateFrom'
                            },
                            to: {
                                name: 'DateTo'
                            }
                        }
                    }
                },
                {
                    name: 'Checked',
                    displayName: $translate.instant('Admin.Js.Reviews.Checked'),
                    width: 80,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                            '<label class="ui-grid-custom-edit-field adv-checkbox-label"> ' +
                                '<input type="checkbox" class="adv-checkbox-input" ng-model="row.entity.Checked" disabled /> ' +
                                '<span class="adv-checkbox-emul"></span> ' +
                            '</label>' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.Checked'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [{ label: $translate.instant('Admin.Js.Reviews.Yes'), value: true }, { label: $translate.instant('Admin.Js.Reviews.No'), value: false }]
                    }
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity.ReviewId)"></a> ' +
                            '<ui-grid-custom-delete url="reviews/deleteReview" params="{\'reviewId\': row.entity.ReviewId }"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.openModal(row.entity.ReviewId);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Reviews.DeleteSelected'),
                        url: 'reviews/deleteReviews',
                        field: 'ReviewId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Reviews.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Reviews.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]
            }
        });


        ctrl.$onInit = function () {

            var locationSearch = $location.search();

            if (locationSearch != null) {

                if (locationSearch.modal != null) {
                    ctrl.openModal(locationSearch.modal);
                }
            }
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.truncate = function(str) {
            if (str.length > 120) {
                str = str.substring(0, 120) + "..";
            }
            return str;
        }

        ctrl.openModal = function (reviewId) {
            $uibModal.open({
                bindToController: true,
                controller: 'ModalAddEditReviewCtrl',
                controllerAs: 'ctrl',
                size: 'lg',
                templateUrl: '../areas/admin/content/src/reviews/modal/addEditReview/addEditReview.html',
                resolve: {
                    reviewId: function () {
                        return reviewId;
                    }
                }
            }).result.then(function (result) {
                $location.search('modal', null);
                ctrl.grid.fetchData();
                return result;
            }, function (result) {
                $location.search('modal', null);
                ctrl.grid.fetchData();
                return result;
            });

            if (reviewId) {
                $location.search('modal', reviewId);
            }

        };
        
    };

    ReviewsCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', '$q', '$uibModal', '$translate'];


    ng.module('reviews', ['uiGridCustom', 'urlHelper'])
      .controller('ReviewsCtrl', ReviewsCtrl);

})(window.angular);