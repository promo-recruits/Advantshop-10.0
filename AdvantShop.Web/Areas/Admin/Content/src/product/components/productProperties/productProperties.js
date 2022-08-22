; (function (ng) {
    'use strict';

    var ProductPropertiesCtrl = function ($http, $filter, $q, $timeout, toaster, $translate, productPropertiesService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getCurrentProperties()
            //.then(ctrl.selectProperty);

            ctrl.propertiesPage = 0;
            ctrl.propertiesSize = 200;
            ctrl.propertiesTotalPageCount = 0;

            ctrl.propertyValuesPage = 0;
            ctrl.propertyValuesSize = 200;
            ctrl.propertyValuesTotalPageCount = 0;

            ctrl.propertiesList = [];

            ctrl.propertyValuesList = [];

            ctrl.pagingForExistPropeties = {};

            ctrl.propertiesDirty = false;
            ctrl.propertiesValuesDirty = false;
        };

        ctrl.propertyValueTransform = function (newTag) {
            return {
                Value: newTag,
                isTag: true
            };
        };

        ctrl.addPropertyValue = function (property, item, model) {

            var params = {
                ProductId: ctrl.productId,
                PropertyId: property.PropertyId,
                PropertyValueId: item.PropertyValueId,
                Value: item.Value,
                IsNew: isNaN(parseFloat(item.PropertyValueId)) || parseFloat(item.PropertyValueId) < 1 ? true : false
            };

            if (property.SelectedPropertyValues.filter(function (child) { return child.Value.toLowerCase() === model.Value.toLowerCase(); }).length > 1) {
                return;
            }

            productPropertiesService.addPropertyValue(params)
                .then(function (data) {
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSuccessfullySaved'));
                        item.PropertyValueId = data.propertyValueId;
                        model.PropertyValueId = data.propertyValueId;
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileAddingProperty'));
                    }
                });
        };

        ctrl.addPropertyWithValue = function (FormCtrl) {

            var params = { ProductId: ctrl.productId };

            if (ctrl.selectedPropertyId != null) {
                params.PropertyId = ctrl.selectedPropertyId;
            } else {
                params.PropertyName = ctrl.selectedProperty.Name;
            }

            if (ctrl.selectedPropertyId != null && ctrl.selectedPropertyValueId != null) {
                params.PropertyValueId = ctrl.selectedPropertyValueId;
            } else {
                params.PropertyValue = ctrl.selectedPropertyValue.Value;
            }

            productPropertiesService.addPropertyWithValue(params)
                .then(function (data) {
                    if (data.result === true) {

                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSuccessfullySaved'));

                        ctrl.getCurrentProperties()
                        //.then(ctrl.selectProperty);

                        if (ctrl.$selectProperty) {
                            ctrl.$selectProperty.search = '';
                        }

                        if (ctrl.$selectPropertyValue) {
                            ctrl.$selectPropertyValue.search = '';
                        }

                        ctrl.selectedProperty = null;
                        ctrl.selectedPropertyId = null;

                        ctrl.selectedPropertyValue = null;
                        ctrl.selectedPropertyValueId = null;

                        ctrl.propertiesQ = null;

                        FormCtrl.$setPristine();
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileAddingProperty'));
                    }
                });
        };

        ctrl.removePropertyValue = function (propertyId, item, model, groupId) {

            var params = {
                ProductId: ctrl.productId,
                PropertyValueId: item.PropertyValueId,
            };

            ctrl.pagingForExistPropeties[propertyId].page = 0;

            productPropertiesService.removePropertyValue(params)
                .then(function (data) {
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSuccessfullySaved'));
                        ctrl.getCurrentProperties()
                        //.then(ctrl.selectProperty);
                    }
                });
        };

        ctrl.selectProperty = function ($item, $model) {

            ctrl.selectedPropertyId = $item.PropertyId;

            ctrl.selectedPropertyValue = null;
            ctrl.selectedPropertyValueId = null;

            ctrl.propertiesQ = null;

            if (ctrl.$selectProperty) {
                ctrl.$selectProperty.search = $model.Name;
            }

            if (ctrl.$selectPropertyValue) {
                ctrl.$selectPropertyValue.search = '';
            }

            ctrl.propertyValuesList.length = 0;

            ctrl.findPropertyValue(ctrl.selectedPropertyId, null);
        };

        ctrl.selectPropertyValue = function ($item, $model) {

            if (ctrl.$selectPropertyValue) {
                ctrl.$selectPropertyValue.search = $model != null ? $model.Value : '';
            }

            ctrl.propertyValuesQ = null;

            ctrl.selectedPropertyValueId = $model != null ? $model.PropertyValueId : null;
        };

        ctrl.findProperty = function (q, $select) {
            ctrl.$selectProperty = $select;

            ctrl.propertiesQ = q;
            ctrl.propertiesPage = 1;

            if (ctrl.$selectPropertyValue) {
                ctrl.$selectPropertyValue.search = '';
            }

            ctrl.selectedPropertyValue = null;
            ctrl.selectedPropertyValueId = null;

            ctrl.propertyValuesList.length = 0;

            productPropertiesService.getAllProperties(ctrl.propertiesPage, ctrl.propertiesSize, q)
                .then(function (data) {

                    var hasItems = data.DataItems.length > 0;
                    var result = hasItems === true ? data.DataItems : [];
                    var qItem = { Name: q };
                    var itemFinded;

                    if (q != null && q.length > 0) {
                        for (var i = 0, len = data.DataItems.length; i < len; i++) {
                            if (q === data.DataItems[i].Name) {
                                itemFinded = data.DataItems[i];
                            }
                        }
                    }

                    if (itemFinded != null) {
                        ctrl.selectedProperty = itemFinded;
                        ctrl.selectedPropertyId = itemFinded.PropertyId;
                    } else if (q != null && q.length > 0) {
                        ctrl.selectedProperty = qItem;
                        ctrl.selectedPropertyId = null;
                        result.push(qItem);
                    }


                    return ctrl.propertiesList = result;
                });
        };

        ctrl.findPropertyValue = function (propertyId, q, $select) {
            var defer = $q.defer();
            var promise;

            ctrl.$selectPropertyValue = $select;

            ctrl.propertyValuesQ = q;
            ctrl.propertyValuesPage = 1;

            if (propertyId != null) {
                promise = productPropertiesService.getAllPropertyValues(propertyId, ctrl.propertyValuesPage, ctrl.propertyValuesSize, q)
                    .then(function (data) {
                        var hasItems = data.DataItems.length > 0;
                        var result = hasItems === true ? data.DataItems : [];
                        var qItem = { Value: q };
                        var itemFinded;

                        if (q != null && q.length > 0) {
                            for (var i = 0, len = data.DataItems.length; i < len; i++) {
                                if (q === data.DataItems[i].Value) {
                                    itemFinded = data.DataItems[i];
                                }
                            }
                        }

                        if (itemFinded != null) {
                            ctrl.selectedPropertyValue = itemFinded;
                            ctrl.selectedPropertyValueId = itemFinded.PropertyValueId;
                        } else if (q != null && q.length > 0) {
                            ctrl.selectedPropertyValue = qItem;
                            ctrl.selectedPropertyValueId = null;
                            result.push(qItem);
                        }

                        return ctrl.propertyValuesList = result;
                    });
            } else {
                ctrl.propertyValuesList = q != null && q.length > 0 ? [{ Value: q }] : [];
                promise = defer.promise;
                defer.resolve(ctrl.propertyValuesList);
            }

            return promise;
        };

        ctrl.getCurrentProperties = function () {
            return productPropertiesService.getCurrentProperties(ctrl.productId)
                .then(function (data) {
                    if (data != null) {
                        ctrl.categoryName = data.CategoryName;
                        ctrl.groups = data.Groups;
                    }
                });
        };

        ctrl.getPropertyValuesByProperty = function (property, q) {
            productPropertiesService.getAllPropertyValues(property.PropertyId, ctrl.propertyValuesPage, ctrl.propertyValuesSize, q)
                .then(function (data) {
                    return property.PropertyValues = data.DataItems.length > 0 ? data.DataItems : [{ Value: q }];
                });
        };

        ctrl.getMore = function () {

            if (ctrl.propertiesPage > ctrl.propertiesTotalPageCount || ctrl.loadingProperties === true) {
                return $q.resolve();
            }

            ctrl.propertiesPage += 1;
            ctrl.loadingProperties = true;

            return productPropertiesService.getAllProperties(ctrl.propertiesPage, ctrl.propertiesSize, ctrl.propertiesQ)
                .then(function (data) {
                    ctrl.propertiesList = ctrl.propertiesList.concat(data.DataItems);
                    ctrl.propertiesTotalPageCount = data.TotalPageCount;

                    return data;
                })
                .finally(function () {
                    ctrl.loadingProperties = false;
                });
        };

        ctrl.getMorePropertiesValue = function () {

            if (ctrl.propertyValuesPage > ctrl.propertyValuesTotalPageCount || ctrl.loadingPropertyValues === true) {
                return $q.resolve();
            }

            if (ctrl.selectedProperty.PropertyId != null) {
                ctrl.propertyValuesPage += 1;
                ctrl.loadingPropertyValues = true;

                return productPropertiesService.getAllPropertyValues(ctrl.selectedProperty.PropertyId, ctrl.propertyValuesPage, ctrl.propertyValuesSize, ctrl.propertyValuesQ)
                    .then(function (data) {
                        ctrl.propertyValuesList = ctrl.propertyValuesList.concat(data.DataItems);
                        ctrl.propertyValuesTotalPageCount = data.TotalPageCount;

                        return data;
                    })
                    .finally(function () {
                        ctrl.loadingPropertyValues = false;
                    });
            } else {
                return $q.resolve();
            }
        };

        ctrl.getMoreValuesForExistProperty = function (item, size, q) {
            var propertyId = item.PropertyId;

            ctrl.pagingForExistPropeties[propertyId] = ctrl.pagingForExistPropeties[propertyId] || {};

            var currentPage = ctrl.pagingForExistPropeties[propertyId].page || 0;
            var totalPageCount = ctrl.pagingForExistPropeties[propertyId].totalPageCount;
            var newPage;

            if (currentPage >= totalPageCount || ctrl.loadingValuesForExistProperty === true) {
                return $q.resolve();
            }

            if (propertyId != null) {
                newPage = currentPage + 1;
                ctrl.loadingValuesForExistProperty = true;

                return productPropertiesService.getAllPropertyValues(propertyId, newPage, size, q)
                    .then(function (data) {

                        if (data.DataItems != null && data.DataItems.length > 0) {
                            item.PropertyValues = (item.PropertyValues != null ? item.PropertyValues.concat(data.DataItems) : data.DataItems).filter(function (iteration) {
                                return item.SelectedPropertyValues == null ||
                                    item.SelectedPropertyValues.length === 0 ||
                                    !item.SelectedPropertyValues.some(function (child) {
                                        return child.Value.toLowerCase() === iteration.Value.toLowerCase();
                                    });
                            });

                            ctrl.pagingForExistPropeties[propertyId].page = newPage;
                            ctrl.pagingForExistPropeties[propertyId].totalPageCount = data.TotalPageCount;
                        }

                        return data;
                    })
                    .finally(function () {
                        ctrl.loadingValuesForExistProperty = false;
                    });
            } else {
                return $q.resolve();
            }
        };

        ctrl.closeSelectProperty = function (isOpen) {
            //применяем к модели несуществующее свойство
            var propertyInList;

            if (isOpen == false) {

                if (ctrl.propertiesQ != null && ctrl.propertiesQ.length > 0) {
                    for (var i = 0, len = ctrl.propertiesList.length; i < len; i++) {
                        if (ctrl.propertiesList[i].Name.toLowerCase() === ctrl.propertiesQ.toLowerCase() && (ctrl.selectedProperty != null ? ctrl.selectedProperty === ctrl.propertiesList[i] : true)) {
                            propertyInList = ctrl.propertiesList[i];
                            break;
                        }
                    }
                }

                if (ctrl.selectedProperty == null && ctrl.propertiesQ != null && propertyInList == null) {
                    ctrl.selectedProperty = {
                        Name: ctrl.propertiesQ
                    };

                    ctrl.propertyValuesList.length = 0;
                }

                if (propertyInList != null) {
                    ctrl.selectedProperty = propertyInList;
                    ctrl.selectedPropertyId = propertyInList.PropertyId;
                }

                ctrl.propertiesPage = 0;
                ctrl.propertiesQ = null;
            }
        };

        ctrl.closeSelectPropertyValue = function (isOpen) {

            var propertyValueInList;

            if (isOpen == false) {

                if (ctrl.propertyValuesQ != null && ctrl.propertyValuesQ.length > 0) {
                    for (var i = 0, len = ctrl.propertyValuesList.length; i < len; i++) {
                        if (ctrl.propertyValuesList[i].Value.toLowerCase() === ctrl.propertyValuesQ.toLowerCase() && (ctrl.selectedPropertyValue != null ? ctrl.selectedPropertyValue === ctrl.propertyValuesList[i] : true)) {
                            propertyValueInList = ctrl.propertyValuesList[i];
                            break;
                        }
                    }
                }
                //применяем к модели несуществующее свойство
                if (ctrl.selectedPropertyValue == null && ctrl.propertyValuesQ != null && propertyValueInList == null) {
                    ctrl.selectedPropertyValue = {
                        Value: ctrl.propertyValuesQ
                    };
                }


                if (propertyValueInList != null) {
                    ctrl.selectedPropertyValue = propertyValueInList;
                    ctrl.selectedPropertyValueId = propertyValueInList.PropertyValueId;
                }

                ctrl.propertyValuesPage = 0;
                ctrl.propertyValuesQ = null;
            }
        };

        ctrl.closeSelectPropertyExit = function (isOpen, property) {
            if (isOpen === false) {
                ctrl.pagingForExistPropeties[property.PropertyId].page = 0;
                property.PropertyValues.length = 0;
            }
        };

        ctrl.firstCallProperties = function () {
            ctrl.propertiesPage = 0;
            ctrl.propertiesList = [];
            ctrl.propertiesTotalPageCount = 0;
            ctrl.getMore();
        };

        ctrl.firstCallPropertyValues = function () {
            ctrl.propertyValuesPage = 0;
            ctrl.propertyValuesList = [];
            ctrl.propertyValuesTotalPageCount = 0;
            ctrl.getMorePropertiesValue();
        };

        ctrl.firstCallValuesForExistProperty = function (item, size) {
            ctrl.getMoreValuesForExistProperty(item, size);
        };
    };

    ProductPropertiesCtrl.$inject = ['$http', '$filter', '$q', '$timeout', 'toaster', '$translate', 'productPropertiesService'];


    ng.module('productProperties', ['ui.select', 'ui-select-infinity'])
        .controller('ProductPropertiesCtrl', ProductPropertiesCtrl)
        .component('productProperties', {
            templateUrl: '../areas/admin/content/src/product/components/productProperties/productProperties.html',
            controller: 'ProductPropertiesCtrl',
            bindings: {
                productId: '@',
            }
        });

})(window.angular);