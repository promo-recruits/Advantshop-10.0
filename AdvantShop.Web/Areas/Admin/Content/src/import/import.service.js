; (function (ng) {
    'use strict';

    var importService = function ($http) {
        var service = this;

        service.UploadProductFiles = function (exportFeedId, categories) {
            return $http.post('import/uploadproductfiles').then(function (response) {
                return response.data;
            });
        };
                
        service.getFieldsFromCsvFile = function (importSettings) {
            return $http.post('import/GetFieldsFromCsvFile', importSettings).then(function (response) {
                return response.data;
            });
        };

        service.getFieldsFromCategoriesCsvFile = function (importSettings) {
            return $http.post('import/GetFieldsFromCategoriesCsvFile', importSettings).then(function (response) {
                return response.data;
            });
        };

        service.getFieldsFromCustomersCsvFile = function (importSettings) {
            return $http.post('import/GetFieldsFromCustomersCsvFile', importSettings).then(function (response) {
                return response.data;
            });
        };

        service.getFieldsFromLeadsCsvFile = function (importSettings) {
            return $http.post('import/GetFieldsFromLeadsCsvFile', importSettings).then(function (response) {
                return response.data;
            });
        };

        service.getFieldsFromBrandsCsvFile = function (importSettings) {
            return $http.post('import/GetFieldsFromBrandsCsvFile', importSettings).then(function (response) {
                return response.data;
            });
        };

        service.startProductsImport = function (importSettings) {
            return $http.post('import/StartProductsImport', importSettings).then(function (response) {
                return response.data;
            });
        };

        service.startCategoriesImport = function (importSettings) {
            return $http.post('import/StartCategoriesImport', importSettings).then(function (response) {
                return response.data;
            });
        };

        service.startCustomersImport = function (importSettings) {
            return $http.post('import/StartCustomersImport', importSettings).then(function (response) {
                return response.data;
            });
        };

        service.startLeadsImport = function (importSettings) {
            return $http.post('import/StartLeadsImport', importSettings).then(function (response) {
                return response.data;
            });
        };

        service.startBrandsImport = function (importSettings) {
            return $http.post('import/StartBrandsImport', importSettings).then(function (response) {
                return response.data;
            });
        };

        //service.getSaasBlockInformation = function () {
        //    return $http.post('import/GetSaasBlockInformation').then(function (response) {
        //        return response.data;
        //    });
        //};

        service.abortImport = function () {
            return $http.post('ExportImportCommon/InterruptProcess').then(function (response) {
                return response.data;
            });
        };

        //service.getCommonStatistic = function () {
        //    return $http.post('ExportImportCommon/GetCommonStatistic').then(function (response) {
        //        return response.data;
        //    });
        //};

        service.getLogFile = function () {
            $http({
                url: 'ExportImportCommon/GetLogFile',
                method: 'POST',
                params: {},
                headers: {
                    'Content-type': 'application/txt',
                },
                responseType: 'arraybuffer'
            }).success(function (data, status, headers, config) {
                var file = new Blob([data], {
                    type: 'application/txt'
                });
                //trick to download store a file having its URL
                var fileURL = URL.createObjectURL(file);
                var a = document.createElement('a');
                a.href = fileURL;
                a.target = '_blank';
                a.download = 'errlog.txt';
                document.body.appendChild(a);
                a.click();
            }).error(function (data, status, headers, config) {

            });
        };

        service.getExampleCustomersFile = function (importSettings) {
            $http({
                url: 'import/getExampleCustomersFile',
                method: 'POST',
                params: importSettings,
                headers: {
                    'Content-type': 'application/txt',
                },
                responseType: 'arraybuffer'
            }).success(function (data, status, headers, config) {
                var file = new Blob([data], {
                    type: 'application/txt'
                });
                //trick to download store a file having its URL
                var fileURL = URL.createObjectURL(file);
                var a = document.createElement('a');
                a.href = fileURL;
                a.target = '_blank';
                a.download = 'exampleCustomersFile.csv';
                document.body.appendChild(a);
                a.click();
            }).error(function (data, status, headers, config) {

            });
        };

        service.getExampleLeadsFile = function (importSettings) {
            $http({
                url: 'import/getExampleLeadsFile',
                method: 'POST',
                params: importSettings,
                headers: {
                    'Content-type': 'application/txt',
                },
                responseType: 'arraybuffer'
            }).success(function (data, status, headers, config) {
                var file = new Blob([data], {
                    type: 'application/txt'
                });
                //trick to download store a file having its URL
                var fileURL = URL.createObjectURL(file);
                var a = document.createElement('a');
                a.href = fileURL;
                a.target = '_blank';
                a.download = 'exampleLeadsFile.csv';
                document.body.appendChild(a);
                a.click();
            }).error(function (data, status, headers, config) {

            });
        };

        service.getExampleBrandsFile = function (importSettings) {
            $http({
                url: 'import/getExampleBrandsFile',
                method: 'POST',
                params: importSettings,
                headers: {
                    'Content-type': 'application/txt',
                },
                responseType: 'arraybuffer'
            }).success(function (data, status, headers, config) {
                var file = new Blob([data], {
                    type: 'application/txt'
                });
                //trick to download store a file having its URL
                var fileURL = URL.createObjectURL(file);
                var a = document.createElement('a');
                a.href = fileURL;
                a.target = '_blank';
                a.download = 'exampleBrandsFile.csv';
                document.body.appendChild(a);
                a.click();
            }).error(function (data, status, headers, config) {

            });
        };
    };

    importService.$inject = ['$http'];

    ng.module('import')
        .service('importService', importService);

})(window.angular);