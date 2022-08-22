; (function (ng) {
    'use strict';

    var FilesCtrl = function ($location, $window, uiGridConstants, uiGridCustomConfig, $q, SweetAlert, $http, $translate) {

        var ctrl = this,
            columnDefs = [
                {
                    name: 'FileName',
                    displayName: $translate.instant('Admin.Js.Files.FileName'),
                    enableSorting: true,
                    enableCellEdit: true,
                },
                {
                    name: 'FileSizeString',
                    displayName: $translate.instant('Admin.Js.Files.FileSize'),
                    enableSorting: true,
                    enableCellEdit: false,
                    width: 150,

                },
                {
                    name: 'DateCreatedString',
                    displayName: $translate.instant('Admin.Js.Files.CreatingDate'),
                    enableSorting: true,
                    enableCellEdit: false,
                    width: 150,
                },
                 {
                     name: 'DateModifiedString',
                     displayName: $translate.instant('Admin.Js.Files.EditingDate'),
                     enableSorting: true,
                     enableCellEdit: false,
                     width: 150,
                 },
                {
                    name: 'Link',
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="{{row.entity.Link}}" download>' + $translate.instant('Admin.Js.Files.Download') + '</a></div>"',
                    displayName: $translate.instant('Admin.Js.Files.Link'),
                    enableSorting: false,
                    enableCellEdit: false,
                    width: 100,
                    
                },

                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                            '<ui-grid-custom-delete url="Files/DeleteFile" params="{\'Ids\': row.entity.FileName}"></ui-grid-custom-delete>' +
                        '</div></div>'
                }
            ];


        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Files.DeleteSelected'),
                        url: 'Files/DeleteFile',
                        field: 'FileName',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Files.AreYouSureDelete'), { title: $translate.instant('Admin.Js.Files.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },
                ]
            }
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.upload = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: '/Files/UploadFile',
                    data: {
                        file: $file,
                        rnd: Math.random(),
                    }
                }).then(function (response) {
                    var data = response.data;
                    if (data.Result == true) {
                        ctrl.ImageSrc = data.Picture;
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Files.ErrorWhileLoadingFile'), data.error);
                    }
                })
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Files.ErrorWhileLoadingFile'), $translate.instant('Admin.Js.Files.FileDoesNotMeet'));
            }
        };

        ctrl.fetchData = function () {
            ctrl.grid.fetchData();
            return {
                result: true
            };
        };
    };

    FilesCtrl.$inject = ['$location', '$window', 'uiGridConstants', 'uiGridCustomConfig', '$q', 'SweetAlert', '$http', '$translate'];


    ng.module('files', ['uiGridCustom', 'urlHelper'])
      .controller('FilesCtrl', FilesCtrl);

})(window.angular);