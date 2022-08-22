; (function (ng) {
    'use strict';

    var PropertyGroupsCtrl = function ($http, SweetAlert, toaster, urlHelper, $window, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            
            ctrl.fetch();

            if (ctrl.onInit != null) {
                ctrl.onInit({ propertyGroups: ctrl });
            }
        };

        ctrl.fetch = function () {
            $http.get('properties/getGroups').then(function (response) {
                ctrl.groups = response.data;
            });
        };

        ctrl.deleteGroup = function (id, index) {
            SweetAlert.confirm($translate.instant('Admin.Js.Properties.AreYouSureDeleteGroup'), { title: $translate.instant('Admin.Js.Properties.DeletingGroup') })
                .then(function(result) {
                    if (result === true) {
                        $http.post('properties/deleteGroup', { propertyGroupId: id }).then(function(response) {

                            //var groupId = urlHelper.getUrlParam('groupId');
                            //if (groupId == id) {
                            //    $window.location.assign('properties');
                            //}
                            //ctrl.fetch();
                            ctrl.selectGroup(0);
                            ctrl.groups.splice(index, 1);
                        });
                    }
                });
        }

        ctrl.sortableOptions = {
            orderChanged: function(event) {
                var propertyGroupId = event.source.itemScope.group.PropertyGroupId,
                    prevGroup = ctrl.groups[event.dest.index - 1],
                    nextGroup = ctrl.groups[event.dest.index + 1];
                
                $http.post('properties/changeGroupSorting', {
                    groupId: propertyGroupId,
                    prevGroupId: prevGroup != null ? prevGroup.PropertyGroupId : null,
                    nextGroupId: nextGroup != null ? nextGroup.PropertyGroupId : null
                }).then(function(response) {
                    if (response.data.result == true) {
                        toaster.pop("success", "", $translate.instant('Admin.Js.Properties.ChangesSaved'));
                    }
                });
            }
        };

        ctrl.selectGroup = function(groupId) {
            ctrl.groupId = groupId;
            if (ctrl.onChangeGroup != null) {
                ctrl.onChangeGroup({ groupId: ctrl.groupId });
            }
        }

    };

    PropertyGroupsCtrl.$inject = ['$http', 'SweetAlert', 'toaster', 'urlHelper', '$window', '$translate'];

    ng.module('propertyGroups', ['as.sortable'])
        .controller('PropertyGroupsCtrl', PropertyGroupsCtrl)
        .component('propertyGroups', {
            templateUrl: '../areas/admin/content/src/properties/components/propertyGroups/templates/groups.html',
            controller: 'PropertyGroupsCtrl',
            transclude: true,
            bindings: {
                groupId: '<?',
                onInit: '&',
                onChangeGroup: '&'
            }
        });

})(window.angular);