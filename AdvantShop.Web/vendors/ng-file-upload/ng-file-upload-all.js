/**!
 * AngularJS file upload/drop directive and service with progress and abort
 * FileAPI Flash shim for old browsers not supporting FormData
 * @author  Danial  <danial.farid@gmail.com>
 * @version 6.1.0
 */

(function () {
  /** @namespace FileAPI.noContentTimeout */

  function patchXHR(fnName, newFn) {
    window.XMLHttpRequest.prototype[fnName] = newFn(window.XMLHttpRequest.prototype[fnName]);
  }

  function redefineProp(xhr, prop, fn) {
    try {
      Object.defineProperty(xhr, prop, {get: fn});
    } catch (e) {/*ignore*/
    }
  }

  if (!window.FileAPI) {
    window.FileAPI = {};
  }

  FileAPI.shouldLoad = (window.XMLHttpRequest && !window.FormData) || FileAPI.forceLoad;
  if (FileAPI.shouldLoad) {
    var initializeUploadListener = function (xhr) {
      if (!xhr.__listeners) {
        if (!xhr.upload) xhr.upload = {};
        xhr.__listeners = [];
        var origAddEventListener = xhr.upload.addEventListener;
        xhr.upload.addEventListener = function (t, fn) {
          xhr.__listeners[t] = fn;
          if (origAddEventListener) origAddEventListener.apply(this, arguments);
        };
      }
    };

    patchXHR('open', function (orig) {
      return function (m, url, b) {
        initializeUploadListener(this);
        this.__url = url;
        try {
          orig.apply(this, [m, url, b]);
        } catch (e) {
          if (e.message.indexOf('Access is denied') > -1) {
            this.__origError = e;
            orig.apply(this, [m, '_fix_for_ie_crossdomain__', b]);
          }
        }
      };
    });

    patchXHR('getResponseHeader', function (orig) {
      return function (h) {
        return this.__fileApiXHR && this.__fileApiXHR.getResponseHeader ? this.__fileApiXHR.getResponseHeader(h) : (orig == null ? null : orig.apply(this, [h]));
      };
    });

    patchXHR('getAllResponseHeaders', function (orig) {
      return function () {
        return this.__fileApiXHR && this.__fileApiXHR.getAllResponseHeaders ? this.__fileApiXHR.getAllResponseHeaders() : (orig == null ? null : orig.apply(this));
      };
    });

    patchXHR('abort', function (orig) {
      return function () {
        return this.__fileApiXHR && this.__fileApiXHR.abort ? this.__fileApiXHR.abort() : (orig == null ? null : orig.apply(this));
      };
    });

    patchXHR('setRequestHeader', function (orig) {
      return function (header, value) {
        if (header === '__setXHR_') {
          initializeUploadListener(this);
          var val = value(this);
          // fix for angular < 1.2.0
          if (val instanceof Function) {
            val(this);
          }
        } else {
          this.__requestHeaders = this.__requestHeaders || {};
          this.__requestHeaders[header] = value;
          orig.apply(this, arguments);
        }
      };
    });

    patchXHR('send', function (orig) {
      return function () {
        var xhr = this;
        if (arguments[0] && arguments[0].__isFileAPIShim) {
          var formData = arguments[0];
          var config = {
            url: xhr.__url,
            jsonp: false, //removes the callback form param
            cache: true, //removes the ?fileapiXXX in the url
            complete: function (err, fileApiXHR) {
              xhr.__completed = true;
              if (!err && xhr.__listeners.load)
                xhr.__listeners.load({
                  type: 'load',
                  loaded: xhr.__loaded,
                  total: xhr.__total,
                  target: xhr,
                  lengthComputable: true
                });
              if (!err && xhr.__listeners.loadend)
                xhr.__listeners.loadend({
                  type: 'loadend',
                  loaded: xhr.__loaded,
                  total: xhr.__total,
                  target: xhr,
                  lengthComputable: true
                });
              if (err === 'abort' && xhr.__listeners.abort)
                xhr.__listeners.abort({
                  type: 'abort',
                  loaded: xhr.__loaded,
                  total: xhr.__total,
                  target: xhr,
                  lengthComputable: true
                });
              if (fileApiXHR.status !== undefined) redefineProp(xhr, 'status', function () {
                return (fileApiXHR.status === 0 && err && err !== 'abort') ? 500 : fileApiXHR.status;
              });
              if (fileApiXHR.statusText !== undefined) redefineProp(xhr, 'statusText', function () {
                return fileApiXHR.statusText;
              });
              redefineProp(xhr, 'readyState', function () {
                return 4;
              });
              if (fileApiXHR.response !== undefined) redefineProp(xhr, 'response', function () {
                return fileApiXHR.response;
              });
              var resp = fileApiXHR.responseText || (err && fileApiXHR.status === 0 && err !== 'abort' ? err : undefined);
              redefineProp(xhr, 'responseText', function () {
                return resp;
              });
              redefineProp(xhr, 'response', function () {
                return resp;
              });
              if (err) redefineProp(xhr, 'err', function () {
                return err;
              });
              xhr.__fileApiXHR = fileApiXHR;
              if (xhr.onreadystatechange) xhr.onreadystatechange();
              if (xhr.onload) xhr.onload();
            },
            progress: function (e) {
              e.target = xhr;
              if (xhr.__listeners.progress) xhr.__listeners.progress(e);
              xhr.__total = e.total;
              xhr.__loaded = e.loaded;
              if (e.total === e.loaded) {
                // fix flash issue that doesn't call complete if there is no response text from the server
                var _this = this;
                setTimeout(function () {
                  if (!xhr.__completed) {
                    xhr.getAllResponseHeaders = function () {
                    };
                    _this.complete(null, {status: 204, statusText: 'No Content'});
                  }
                }, FileAPI.noContentTimeout || 10000);
              }
            },
            headers: xhr.__requestHeaders
          };
          config.data = {};
          config.files = {};
          for (var i = 0; i < formData.data.length; i++) {
            var item = formData.data[i];
            if (item.val != null && item.val.name != null && item.val.size != null && item.val.type != null) {
              config.files[item.key] = item.val;
            } else {
              config.data[item.key] = item.val;
            }
          }

          setTimeout(function () {
            if (!FileAPI.hasFlash) {
              throw 'Adode Flash Player need to be installed. To check ahead use "FileAPI.hasFlash"';
            }
            xhr.__fileApiXHR = FileAPI.upload(config);
          }, 1);
        } else {
          if (this.__origError) {
            throw this.__origError;
          }
          orig.apply(xhr, arguments);
        }
      };
    });
    window.XMLHttpRequest.__isFileAPIShim = true;
    window.FormData = FormData = function () {
      return {
        append: function (key, val, name) {
          if (val.__isFileAPIBlobShim) {
            val = val.data[0];
          }
          this.data.push({
            key: key,
            val: val,
            name: name
          });
        },
        data: [],
        __isFileAPIShim: true
      };
    };

    window.Blob = Blob = function (b) {
      return {
        data: b,
        __isFileAPIBlobShim: true
      };
    };
  }

})();

(function () {
  /** @namespace FileAPI.forceLoad */
  /** @namespace window.FileAPI.jsUrl */
  /** @namespace window.FileAPI.jsPath */

  function isInputTypeFile(elem) {
    return elem[0].tagName.toLowerCase() === 'input' && elem.attr('type') && elem.attr('type').toLowerCase() === 'file';
  }

  function hasFlash() {
    try {
      var fo = new ActiveXObject('ShockwaveFlash.ShockwaveFlash');
      if (fo) return true;
    } catch (e) {
      if (navigator.mimeTypes['application/x-shockwave-flash'] !== undefined) return true;
    }
    return false;
  }

  function getOffset(obj) {
    var left = 0, top = 0;

    if (window.jQuery) {
      return jQuery(obj).offset();
    }

    if (obj.offsetParent) {
      do {
        left += (obj.offsetLeft - obj.scrollLeft);
        top += (obj.offsetTop - obj.scrollTop);
        obj = obj.offsetParent;
      } while (obj);
    }
    return {
      left: left,
      top: top
    };
  }

  if (FileAPI.shouldLoad) {

    //load FileAPI
    if (FileAPI.forceLoad) {
      FileAPI.html5 = false;
    }

    if (!FileAPI.upload) {
      var jsUrl, basePath, script = document.createElement('script'), allScripts = document.getElementsByTagName('script'), i, index, src;
      if (window.FileAPI.jsUrl) {
        jsUrl = window.FileAPI.jsUrl;
      } else if (window.FileAPI.jsPath) {
        basePath = window.FileAPI.jsPath;
      } else {
        for (i = 0; i < allScripts.length; i++) {
          src = allScripts[i].src;
          index = src.search(/\/ng\-file\-upload[\-a-zA-z0-9\.]*\.js/);
          if (index > -1) {
            basePath = src.substring(0, index + 1);
            break;
          }
        }
      }

      if (FileAPI.staticPath == null) FileAPI.staticPath = basePath;
      script.setAttribute('src', jsUrl || basePath + 'FileAPI.min.js');
      document.getElementsByTagName('head')[0].appendChild(script);

      FileAPI.hasFlash = hasFlash();
    }

    FileAPI.ngfFixIE = function (elem, createFileElemFn, bindAttr, changeFn) {
      if (!hasFlash()) {
        throw 'Adode Flash Player need to be installed. To check ahead use "FileAPI.hasFlash"';
      }
      var makeFlashInput = function () {
        if (elem.attr('disabled')) {
          elem.$$ngfRefElem.removeClass('js-fileapi-wrapper');
        } else {
          var fileElem = elem.$$ngfRefElem;
          if (!fileElem) {
            fileElem = elem.$$ngfRefElem = createFileElemFn();
            fileElem.addClass('js-fileapi-wrapper');
            if (!isInputTypeFile(elem)) {
//						if (fileElem.parent().css('position') === '' || fileElem.parent().css('position') === 'static') {
//							fileElem.parent().css('position', 'relative');
//						}
//						elem.parent()[0].insertBefore(fileElem[0], elem[0]);
//						elem.css('overflow', 'hidden');
            }
            setTimeout(function () {
              fileElem.bind('mouseenter', makeFlashInput);
            }, 10);
            fileElem.bind('change', function (evt) {
              fileApiChangeFn.apply(this, [evt]);
              changeFn.apply(this, [evt]);
//						alert('change' +  evt);
            });
          } else {
            bindAttr(elem.$$ngfRefElem);
          }
          if (!isInputTypeFile(elem)) {
              fileElem.css('position', 'fixed')
              .css('top', getOffset(elem[0]).top + 'px').css('left', getOffset(elem[0]).left + 'px')
              .css('width', elem[0].offsetWidth + 'px').css('height', elem[0].offsetHeight + 'px')
              .css('filter', 'alpha(opacity=0)').css('display', elem.css('display'))
              .css('overflow', 'hidden').css('z-index', '900000')
              .css('visibility', 'visible');
          }
        }
      };

      elem.bind('mouseenter', makeFlashInput);

      var fileApiChangeFn = function (evt) {
        var files = FileAPI.getFiles(evt);
        //just a double check for #233
        for (var i = 0; i < files.length; i++) {
          if (files[i].size === undefined) files[i].size = 0;
          if (files[i].name === undefined) files[i].name = 'file';
          if (files[i].type === undefined) files[i].type = 'undefined';
        }
        if (!evt.target) {
          evt.target = {};
        }
        evt.target.files = files;
        // if evt.target.files is not writable use helper field
        if (evt.target.files !== files) {
          evt.__files_ = files;
        }
        (evt.__files_ || evt.target.files).item = function (i) {
          return (evt.__files_ || evt.target.files)[i] || null;
        };
      };
    };

    FileAPI.disableFileInput = function (elem, disable) {
      if (disable) {
        elem.removeClass('js-fileapi-wrapper');
      } else {
        elem.addClass('js-fileapi-wrapper');
      }
    };
  }
})();

if (!window.FileReader) {
  window.FileReader = function () {
    var _this = this, loadStarted = false;
    this.listeners = {};
    this.addEventListener = function (type, fn) {
      _this.listeners[type] = _this.listeners[type] || [];
      _this.listeners[type].push(fn);
    };
    this.removeEventListener = function (type, fn) {
      if (_this.listeners[type]) _this.listeners[type].splice(_this.listeners[type].indexOf(fn), 1);
    };
    this.dispatchEvent = function (evt) {
      var list = _this.listeners[evt.type];
      if (list) {
        for (var i = 0; i < list.length; i++) {
          list[i].call(_this, evt);
        }
      }
    };
    this.onabort = this.onerror = this.onload = this.onloadstart = this.onloadend = this.onprogress = null;

    var constructEvent = function (type, evt) {
      var e = {type: type, target: _this, loaded: evt.loaded, total: evt.total, error: evt.error};
      if (evt.result != null) e.target.result = evt.result;
      return e;
    };
    var listener = function (evt) {
      if (!loadStarted) {
        loadStarted = true;
        if (_this.onloadstart) _this.onloadstart(constructEvent('loadstart', evt));
      }
      var e;
      if (evt.type === 'load') {
        if (_this.onloadend) _this.onloadend(constructEvent('loadend', evt));
        e = constructEvent('load', evt);
        if (_this.onload) _this.onload(e);
        _this.dispatchEvent(e);
      } else if (evt.type === 'progress') {
        e = constructEvent('progress', evt);
        if (_this.onprogress) _this.onprogress(e);
        _this.dispatchEvent(e);
      } else {
        e = constructEvent('error', evt);
        if (_this.onerror) _this.onerror(e);
        _this.dispatchEvent(e);
      }
    };
    this.readAsArrayBuffer = function (file) {
      FileAPI.readAsBinaryString(file, listener);
    };
    this.readAsBinaryString = function (file) {
      FileAPI.readAsBinaryString(file, listener);
    };
    this.readAsDataURL = function (file) {
      FileAPI.readAsDataURL(file, listener);
    };
    this.readAsText = function (file) {
      FileAPI.readAsText(file, listener);
    };
  };
}

/**!
 * AngularJS file upload/drop directive and service with progress and abort
 * @author  Danial  <danial.farid@gmail.com>
 * @version 6.1.0
 */

if (window.XMLHttpRequest && !(window.FileAPI && FileAPI.shouldLoad)) {
  window.XMLHttpRequest.prototype.setRequestHeader = (function (orig) {
    return function (header, value) {
      if (header === '__setXHR_') {
        var val = value(this);
        // fix for angular < 1.2.0
        if (val instanceof Function) {
          val(this);
        }
      } else {
        orig.apply(this, arguments);
      }
    };
  })(window.XMLHttpRequest.prototype.setRequestHeader);
}

var ngFileUpload = angular.module('ngFileUpload', []);

ngFileUpload.version = '6.1.0';
ngFileUpload.defaults = {};

ngFileUpload.service('Upload', ['$http', '$q', '$timeout', function ($http, $q, $timeout) {
  function sendHttp(config) {
    config.method = config.method || 'POST';
    config.headers = config.headers || {};

    var deferred = $q.defer();
    var promise = deferred.promise;

    config.headers.__setXHR_ = function () {
      return function (xhr) {
        if (!xhr) return;
        config.__XHR = xhr;
        if (config.xhrFn) config.xhrFn(xhr);
        xhr.upload.addEventListener('progress', function (e) {
          e.config = config;
          if (deferred.notify) {
            deferred.notify(e);
          } else if (promise.progressFunc) {
            $timeout(function () {
              promise.progressFunc(e);
            });
          }
        }, false);
        //fix for firefox not firing upload progress end, also IE8-9
        xhr.upload.addEventListener('load', function (e) {
          if (e.lengthComputable) {
            e.config = config;
            if (deferred.notify) {
              deferred.notify(e);
            } else if (promise.progressFunc) {
              $timeout(function () {
                promise.progressFunc(e);
              });
            }
          }
        }, false);
      };
    };

    $http(config).then(function (r) {
      deferred.resolve(r);
    }, function (e) {
      deferred.reject(e);
    }, function (n) {
      deferred.notify(n);
    });

    promise.success = function (fn) {
      promise.then(function (response) {
        fn(response.data, response.status, response.headers, config);
      });
      return promise;
    };

    promise.error = function (fn) {
      promise.then(null, function (response) {
        fn(response.data, response.status, response.headers, config);
      });
      return promise;
    };

    promise.progress = function (fn) {
      promise.progressFunc = fn;
      promise.then(null, null, function (update) {
        fn(update);
      });
      return promise;
    };
    promise.abort = function () {
      if (config.__XHR) {
        $timeout(function () {
          config.__XHR.abort();
        });
      }
      return promise;
    };
    promise.xhr = function (fn) {
      config.xhrFn = (function (origXhrFn) {
        return function () {
          if (origXhrFn) origXhrFn.apply(promise, arguments);
          fn.apply(promise, arguments);
        };
      })(config.xhrFn);
      return promise;
    };

    return promise;
  }

  this.upload = function (config) {
    function addFieldToFormData(formData, val, key) {
      if (val !== undefined) {
        if (angular.isDate(val)) {
          val = val.toISOString();
        }
        if (angular.isString(val)) {
          formData.append(key, val);
        } else if (config.sendFieldsAs === 'form') {
          if (angular.isObject(val)) {
            for (var k in val) {
              if (val.hasOwnProperty(k)) {
                addFieldToFormData(formData, val[k], key + '[' + k + ']');
              }
            }
          } else {
            formData.append(key, val);
          }
        } else {
          val = angular.isString(val) ? val : JSON.stringify(val);
          if (config.sendFieldsAs === 'json-blob') {
            formData.append(key, new Blob([val], {type: 'application/json'}));
          } else {
            formData.append(key, val);
          }
        }
      }
    }

    config.headers = config.headers || {};
    config.headers['Content-Type'] = undefined;
    config.transformRequest = config.transformRequest ?
      (angular.isArray(config.transformRequest) ?
        config.transformRequest : [config.transformRequest]) : [];
    config.transformRequest.push(function (data) {
      var formData = new FormData();
      var allFields = {};
      var key;
      for (key in config.fields) {
        if (config.fields.hasOwnProperty(key)) {
          allFields[key] = config.fields[key];
        }
      }
      if (data) allFields.data = data;
      for (key in allFields) {
        if (allFields.hasOwnProperty(key)) {
          var val = allFields[key];
          if (config.formDataAppender) {
            config.formDataAppender(formData, key, val);
          } else {
            addFieldToFormData(formData, val, key);
          }
        }
      }

      if (config.file != null) {
        var fileFormName = config.fileFormDataName || 'file';

        if (angular.isArray(config.file)) {
          var isFileFormNameString = angular.isString(fileFormName);
          for (var i = 0; i < config.file.length; i++) {
            formData.append(isFileFormNameString ? fileFormName : fileFormName[i], config.file[i],
              (config.fileName && config.fileName[i]) || config.file[i].name);
          }
        } else {
          formData.append(fileFormName, config.file, config.fileName || config.file.name);
        }
      }
      return formData;
    });

    return sendHttp(config);
  };

  this.http = function (config) {
    config.transformRequest = config.transformRequest || function (data) {
        if ((window.ArrayBuffer && data instanceof window.ArrayBuffer) || data instanceof Blob) {
          return data;
        }
        return $http.defaults.transformRequest[0](arguments);
      };
    return sendHttp(config);
  };

  this.dataUrl = function (file, callback, disallowObjectUrl) {
    if (window.FileReader && file &&
      (!window.FileAPI || navigator.userAgent.indexOf('MSIE 8') === -1 || file.size < 20000) &&
      (!window.FileAPI || navigator.userAgent.indexOf('MSIE 9') === -1 || file.size < 4000000)) {
      $timeout(function () {
        //prefer URL.createObjectURL for handling refrences to files of all sizes
        //since it doesn??t build a large string in memory
        var URL = window.URL || window.webkitURL;
        if (URL && URL.createObjectURL && !disallowObjectUrl) {
          callback(URL.createObjectURL(file));
        } else {
          var fileReader = new FileReader();
          fileReader.readAsDataURL(file);
          fileReader.onload = function (e) {
            $timeout(function () {
              callback(e.target.result);
            });
          };
        }
      });
    } else {
      callback(null);
    }
  };

  this.setDefaults = function(defaults) {
    ngFileUpload.defaults = defaults || {};
  };
}

]);

(function () {
  ngFileUpload.getAttrWithDefaults = function(attr, name) {
    return attr[name] != null ? attr[name] :
      (ngFileUpload.defaults[name] == null ?
        ngFileUpload.defaults[name] : ngFileUpload.defaults[name].toString());
  };

  var getAttr = ngFileUpload.getAttrWithDefaults;

  ngFileUpload.directive('ngfSelect', ['$parse', '$timeout', '$compile',
    function ($parse, $timeout, $compile) {
      return {
        restrict: 'AEC',
        require: '?ngModel',
        link: function (scope, elem, attr, ngModel) {
          linkFileSelect(scope, elem, attr, ngModel, $parse, $timeout, $compile);
        }
      };
    }]);

  function linkFileSelect(scope, elem, attr, ngModel, $parse, $timeout, $compile) {
    /** @namespace attr.ngfSelect */
    /** @namespace attr.ngfChange */
    /** @namespace attr.ngModel */
    /** @namespace attr.ngModelRejected */
    /** @namespace attr.ngfModel */
    /** @namespace attr.ngfMultiple */
    /** @namespace attr.ngfCapture */
    /** @namespace attr.ngfAccept */
    /** @namespace attr.ngfValidate */
    /** @namespace attr.ngfMaxSize */
    /** @namespace attr.ngfMinSize */
    /** @namespace attr.ngfResetOnClick */
    /** @namespace attr.ngfResetModelOnClick */
    /** @namespace attr.ngfKeep */
    /** @namespace attr.ngfKeepDistinct */

    if (elem.attr('__ngf_gen__')) {
      return;
    }

    scope.$on('$destroy', function () {
      if (elem.$$ngfRefElem) elem.$$ngfRefElem.remove();
    });

    var disabled = false;
    if (getAttr(attr, 'ngfSelect').search(/\W+\$files\W+/) === -1) {
      scope.$watch(getAttr(attr, 'ngfSelect'), function (val) {
        disabled = val === false;
      });
    }
    function isInputTypeFile() {
      return elem[0].tagName.toLowerCase() === 'input' && attr.type && attr.type.toLowerCase() === 'file';
    }

    var isUpdating = false;

    function changeFn(evt) {
      if (!isUpdating) {
        isUpdating = true;
        try {
          var fileList = evt.__files_ || (evt.target && evt.target.files);
          var files = [], rejFiles = [];

          for (var i = 0; i < fileList.length; i++) {
            var file = fileList.item(i);
            if (validate(scope, $parse, attr, file, evt)) {
              files.push(file);
            } else {
              rejFiles.push(file);
            }
          }
          updateModel($parse, $timeout, scope, ngModel, attr,
            getAttr(attr, 'ngfChange') || getAttr(attr, 'ngfSelect'), files, rejFiles, evt);
          if (files.length === 0) evt.target.value = files;
//                if (evt.target && evt.target.getAttribute('__ngf_gen__')) {
//                    angular.element(evt.target).remove();
//                }
        } finally {
          isUpdating = false;
        }
      }
    }

    function bindAttrToFileInput(fileElem) {
      if (getAttr(attr, 'ngfMultiple')) fileElem.attr('multiple', $parse(getAttr(attr, 'ngfMultiple'))(scope));
      if (getAttr(attr, 'ngfCapture')) fileElem.attr('capture', $parse(getAttr(attr, 'ngfCapture'))(scope));
      if (getAttr(attr, 'accept')) fileElem.attr('accept', getAttr(attr, 'accept'));
      for (var i = 0; i < elem[0].attributes.length; i++) {
        var attribute = elem[0].attributes[i];
        if ((isInputTypeFile() && attribute.name !== 'type') ||
          (attribute.name !== 'type' && attribute.name !== 'class' &&
          attribute.name !== 'id' && attribute.name !== 'style')) {
          if (attribute.value == null || attribute.value === '') {
            if (attribute.name === 'required') attribute.value = 'required';
            if (attribute.name === 'multiple') attribute.value = 'multiple';
          }
          fileElem.attr(attribute.name, attribute.value);
        }
      }
    }

    function createFileInput(evt, resetOnClick) {
      if (!resetOnClick && (evt || isInputTypeFile())) return elem.$$ngfRefElem || elem;
      if (elem.$$ngfProgramClick) return elem;

      var fileElem = angular.element('<input type="file">');
      bindAttrToFileInput(fileElem);

      if (isInputTypeFile()) {
        elem.replaceWith(fileElem);
        elem = fileElem;
        fileElem.attr('__ngf_gen__', true);
        $compile(elem)(scope);
      } else {
          fileElem.css('visibility', 'hidden').css('position', 'fixed').css('overflow', 'hidden')
          .css('width', '0px').css('height', '0px').css('border', 'none')
          .css('margin', '0px').css('padding', '0px').attr('tabindex', '-1');
        if (elem.$$ngfRefElem) {
          elem.$$ngfRefElem.remove();
        }
        elem.$$ngfRefElem = fileElem;
        document.body.appendChild(fileElem[0]);
      }

      return fileElem;
    }

    function resetModel(evt) {
      updateModel($parse, $timeout, scope, ngModel, attr,
        getAttr(attr, 'ngfChange') || getAttr(attr, 'ngfSelect'), [], [], evt, true);
    }

    var initialTouchStartY = 0;
    function clickHandler(evt) {
      if (elem.attr('disabled') || disabled) return false;

      if (evt != null) {
        var touches = evt.changedTouches || (evt.originalEvent && evt.originalEvent.changedTouches);
        if (evt.type === 'touchstart') {
          initialTouchStartY = touches ? touches[0].clientY : 0;
          return true; // don't block event default
        } else {
          evt.stopPropagation();
          evt.preventDefault();

          // prevent scroll from triggering event
          if (evt.type === 'touchend') {
            var currentLocation = touches ? touches[0].clientY : 0;
            if (Math.abs(currentLocation - initialTouchStartY) > 20) return false;
          }
        }
      }

      var resetOnClick = $parse(getAttr(attr, 'ngfResetOnClick'))(scope) !== false;
      var fileElem = createFileInput(evt, resetOnClick);

      function clickAndAssign(evt) {
        if (evt && !elem.$$ngfProgramClick) {
          elem.$$ngfProgramClick = true;
          fileElem[0].click();
          $timeout(function() {
            delete elem.$$ngfProgramClick;
          }, 500);
        }
        if ((isInputTypeFile() || !evt) && resetOnClick) {
          elem.bind('click touchstart touchend', clickHandler);
        }
      }

      if (fileElem) {
        if (!evt || resetOnClick) fileElem.bind('change', changeFn);
        if (evt && resetOnClick && $parse(getAttr(attr, 'ngfResetModelOnClick'))(scope) !== false) {
          resetModel(evt);
        }

        // fix for android native browser < 4.4
        if (shouldClickLater(navigator.userAgent)) {
          setTimeout(function () {
            clickAndAssign(evt);
          }, 0);
        } else {
          clickAndAssign(evt);
        }
      }

      return false;
    }

    if (window.FileAPI && window.FileAPI.ngfFixIE) {
      window.FileAPI.ngfFixIE(elem, createFileInput, bindAttrToFileInput, changeFn);
    } else {
      clickHandler();
      //if (!isInputTypeFile()) {
      //  elem.bind('click touchend', clickHandler);
      //}
    }
  }

  function shouldClickLater(ua) {
    // android below 4.4
    var m = ua.match(/Android[^\d]*(\d+)\.(\d+)/);
    if (m && m.length > 2) {
      var v = ngFileUpload.defaults.androidFixMinorVersion || 4;
      return parseInt(m[1]) < 4 || (parseInt(m[1]) === v && parseInt(m[2]) < v);
    }

    // safari on windows
    return ua.indexOf('Chrome') === -1 && /.*Windows.*Safari.*/.test(ua);
  }

  ngFileUpload.validate = function (scope, $parse, attr, file, evt) {
    if (file == null) {
      return false;
    }
    function globStringToRegex(str) {
      if (str.length > 2 && str[0] === '/' && str[str.length - 1] === '/') {
        return str.substring(1, str.length - 1);
      }
      var split = str.split(','), result = '';
      if (split.length > 1) {
        for (var i = 0; i < split.length; i++) {
          result += '(' + globStringToRegex(split[i]) + ')';
          if (i < split.length - 1) {
            result += '|';
          }
        }
      } else {
        if (str.indexOf('.') === 0) {
          str = '*' + str;
        }
        result = '^' + str.replace(new RegExp('[.\\\\+*?\\[\\^\\]$(){}=!<>|:\\' + '-]', 'g'), '\\$&') + '$';
        result = result.replace(/\\\*/g, '.*').replace(/\\\?/g, '.');
      }
      return result;
    }

    var custom = $parse(getAttr(attr, 'ngfValidate'))(scope, {$file: file, $event: evt});
    if (custom != null && (custom === false || custom.length > 0)) {
      file.$error = custom ? custom : 'validate';
      return false;
    }
    var accept = $parse(getAttr(attr, 'ngfAccept'))(scope, {$file: file, $event: evt});
    var fileSizeMax = $parse(getAttr(attr, 'ngfMaxSize'))(scope, {$file: file, $event: evt}) || 9007199254740991;
    var fileSizeMin = $parse(getAttr(attr, 'ngfMinSize'))(scope, {$file: file, $event: evt}) || -1;
    if (accept != null && angular.isString(accept)) {
      var regexp = new RegExp(globStringToRegex(accept), 'gi');
      accept = (file.type != null && regexp.test(file.type.toLowerCase())) ||
        (file.name != null && regexp.test(file.name.toLowerCase()));
      if (!accept) {
        file.$error = 'accept';
        return false;
      }
    } else {
      if (accept === false) {
        file.$error = 'accept';
        return false;
      }
    }
    if (file.size == null) return true;
    if (file.size > fileSizeMax) {
      file.$error = 'maxSize';
      return false;
    }
    if (file.size < fileSizeMin) {
      file.$error = 'minSize';
      return false;
    }
    return true;
  };

  ngFileUpload.updateModel = function ($parse, $timeout, scope, ngModel, attr, fileChange,
                                       files, rejFiles, evt, noDelay) {
    function update() {
      var keep = $parse(getAttr(attr, 'ngfKeep'))(scope);
      if (keep === true) {
        var prevFiles = (ngModel.$modelValue || []).slice(0);
        if (!files || !files.length) {
          files = prevFiles;
        } else if ($parse(getAttr(attr, 'ngfKeepDistinct'))(scope) === true) {
          var len = prevFiles.length;
          for (var i = 0; i < files.length; i++) {
            for (var j = 0; j < len; j++) {
              if (files[i].name === prevFiles[j].name) break;
            }
            if (j === len) {
              prevFiles.push(files[i]);
            }
          }
          files = prevFiles;
        } else {
          files = prevFiles.concat(files);
        }
      }
      var file = files && files.length ? files[0] : null;
      if (ngModel) {
        var singleModel = !$parse(getAttr(attr, 'ngfMultiple'))(scope) && ! getAttr(attr, 'multiple') && !keep;
        $parse(getAttr(attr, 'ngModel')).assign(scope, singleModel ? file : files);
        $timeout(function () {
          if (ngModel) {
            ngModel.$setViewValue(singleModel ? file : (files != null && files.length === 0 ? null : files));
          }
        });
      }
      var ngfModel = getAttr(attr, 'ngfModel');
      if (ngfModel) {
        $parse(ngfModel).assign(scope, files);
      }

      if (getAttr(attr, 'ngModelRejected')) {
        $parse(getAttr(attr, 'ngModelRejected')).assign(scope, rejFiles);
      }
      if (fileChange) {
        $parse(fileChange)(scope, {
          $files: files,
          $file: file,
          $rejectedFiles: rejFiles,
          $event: evt
        });
      }
    }

    if (noDelay) {
      update();
    } else {
      $timeout(function () {
        update();
      });
    }
  };

  var validate = ngFileUpload.validate;
  var updateModel = ngFileUpload.updateModel;

})();

(function () {
  var validate = ngFileUpload.validate;
  var updateModel = ngFileUpload.updateModel;
  var getAttr = ngFileUpload.getAttrWithDefaults;

  ngFileUpload.directive('ngfDrop', ['$parse', '$timeout', '$location', function ($parse, $timeout, $location) {
    return {
      restrict: 'AEC',
      require: '?ngModel',
      link: function (scope, elem, attr, ngModel) {
        linkDrop(scope, elem, attr, ngModel, $parse, $timeout, $location);
      }
    };
  }]);

  ngFileUpload.directive('ngfNoFileDrop', function () {
    return function (scope, elem) {
      if (dropAvailable()) elem.css('display', 'none');
    };
  });

  ngFileUpload.directive('ngfDropAvailable', ['$parse', '$timeout', function ($parse, $timeout) {
    return function (scope, elem, attr) {
      if (dropAvailable()) {
        var fn = $parse(getAttr(attr, 'ngfDropAvailable'));
        $timeout(function () {
          fn(scope);
          if (fn.assign) {
            fn.assign(scope, true);
          }
        });
      }
    };
  }]);

  function linkDrop(scope, elem, attr, ngModel, $parse, $timeout, $location) {
    var available = dropAvailable();
    if (getAttr(attr, 'dropAvailable')) {
      $timeout(function () {
        if (scope[getAttr(attr, 'dropAvailable')]) {
          scope[getAttr(attr, 'dropAvailable')].value = available;
        } else {
          scope[getAttr(attr, 'dropAvailable')] = available;
        }
      });
    }
    if (!available) {
      if ($parse(getAttr(attr, 'ngfHideOnDropNotAvailable'))(scope) === true) {
        elem.css('display', 'none');
      }
      return;
    }

    var disabled = false;
    if (getAttr(attr, 'ngfDrop').search(/\W+\$files\W+/) === -1) {
      scope.$watch(getAttr(attr, 'ngfDrop'), function(val) {
        disabled = val === false;
      });
    }

    var leaveTimeout = null;
    var stopPropagation = $parse(getAttr(attr, 'ngfStopPropagation'));
    var dragOverDelay = 1;
    var actualDragOverClass;

    elem[0].addEventListener('dragover', function (evt) {
      if (elem.attr('disabled') || disabled) return;
      evt.preventDefault();
      if (stopPropagation(scope)) evt.stopPropagation();
      // handling dragover events from the Chrome download bar
      if (navigator.userAgent.indexOf('Chrome') > -1) {
        var b = evt.dataTransfer.effectAllowed;
        evt.dataTransfer.dropEffect = ('move' === b || 'linkMove' === b) ? 'move' : 'copy';
      }
      $timeout.cancel(leaveTimeout);
      if (!scope.actualDragOverClass) {
        actualDragOverClass = calculateDragOverClass(scope, attr, evt);
      }
      elem.addClass(actualDragOverClass);
    }, false);
    elem[0].addEventListener('dragenter', function (evt) {
      if (elem.attr('disabled') || disabled) return;
      evt.preventDefault();
      if (stopPropagation(scope)) evt.stopPropagation();
    }, false);
    elem[0].addEventListener('dragleave', function () {
      if (elem.attr('disabled') || disabled) return;
      leaveTimeout = $timeout(function () {
        elem.removeClass(actualDragOverClass);
        actualDragOverClass = null;
      }, dragOverDelay || 1);
    }, false);
    elem[0].addEventListener('drop', function (evt) {
      if (elem.attr('disabled') || disabled) return;
      evt.preventDefault();
      if (stopPropagation(scope)) evt.stopPropagation();
      elem.removeClass(actualDragOverClass);
      actualDragOverClass = null;
      extractFiles(evt, function (files, rejFiles) {
        updateModel($parse, $timeout, scope, ngModel, attr,
          getAttr(attr, 'ngfChange') || getAttr(attr, 'ngfDrop'), files, rejFiles, evt);
      }, $parse(getAttr(attr, 'ngfAllowDir'))(scope) !== false,
        getAttr(attr, 'multiple') || $parse(getAttr(attr, 'ngfMultiple'))(scope));
    }, false);
    elem[0].addEventListener('paste', function (evt) {
      if (elem.attr('disabled') || disabled) return;
      extractFiles(evt, function (files, rejFiles) {
        updateModel($parse, $timeout, scope, ngModel, attr,
          getAttr(attr, 'ngfChange') || getAttr(attr, 'ngfDrop'), files, rejFiles, evt);
      }, false, getAttr(attr, 'multiple') || $parse(getAttr(attr, 'ngfMultiple'))(scope));
    }, false);

    function calculateDragOverClass(scope, attr, evt) {
      var accepted = true;
      var items = evt.dataTransfer.items;
      if (items != null) {
        for (var i = 0; i < items.length && accepted; i++) {
          accepted = accepted &&
            (items[i].kind === 'file' || items[i].kind === '') &&
            validate(scope, $parse, attr, items[i], evt);
        }
      }
      var clazz = $parse(getAttr(attr, 'ngfDragOverClass'))(scope, {$event: evt});
      if (clazz) {
        if (clazz.delay) dragOverDelay = clazz.delay;
        if (clazz.accept) clazz = accepted ? clazz.accept : clazz.reject;
      }
      return clazz || getAttr(attr, 'ngfDragOverClass') || 'dragover';
    }

    function extractFiles(evt, callback, allowDir, multiple) {
      var files = [], rejFiles = [], processing = 0;

      function addFile(file) {
        if (validate(scope, $parse, attr, file, evt)) {
          files.push(file);
        } else {
          rejFiles.push(file);
        }
      }

      function traverseFileTree(files, entry, path) {
        if (entry != null) {
          if (entry.isDirectory) {
            var filePath = (path || '') + entry.name;
            addFile({name: entry.name, type: 'directory', path: filePath});
            var dirReader = entry.createReader();
            var entries = [];
            processing++;
            var readEntries = function () {
              dirReader.readEntries(function (results) {
                try {
                  if (!results.length) {
                    for (var i = 0; i < entries.length; i++) {
                      traverseFileTree(files, entries[i], (path ? path : '') + entry.name + '/');
                    }
                    processing--;
                  } else {
                    entries = entries.concat(Array.prototype.slice.call(results || [], 0));
                    readEntries();
                  }
                } catch (e) {
                  processing--;
                  console.error(e);
                }
              }, function () {
                processing--;
              });
            };
            readEntries();
          } else {
            processing++;
            entry.file(function (file) {
              try {
                processing--;
                file.path = (path ? path : '') + file.name;
                addFile(file);
              } catch (e) {
                processing--;
                console.error(e);
              }
            }, function () {
              processing--;
            });
          }
        }
      }

      if (evt.type === 'paste') {
        var clipboard = evt.clipboardData || evt.originalEvent.clipboardData;
        if (clipboard && clipboard.items) {
          for (var k = 0; k < clipboard.items.length; k++) {
            if (clipboard.items[k].type.indexOf('image') !== -1) {
              addFile(clipboard.items[k].getAsFile());
            }
          }
          callback(files, rejFiles);
        }
      } else {
        var items = evt.dataTransfer.items;

        if (items && items.length > 0 && $location.protocol() !== 'file') {
          for (var i = 0; i < items.length; i++) {
            if (items[i].webkitGetAsEntry && items[i].webkitGetAsEntry() && items[i].webkitGetAsEntry().isDirectory) {
              var entry = items[i].webkitGetAsEntry();
              if (entry.isDirectory && !allowDir) {
                continue;
              }
              if (entry != null) {
                traverseFileTree(files, entry);
              }
            } else {
              var f = items[i].getAsFile();
              if (f != null) addFile(f);
            }
            if (!multiple && files.length > 0) break;
          }
        } else {
          var fileList = evt.dataTransfer.files;
          if (fileList != null) {
            for (var j = 0; j < fileList.length; j++) {
              addFile(fileList.item(j));
              if (!multiple && files.length > 0) {
                break;
              }
            }
          }
        }
        var delays = 0;
        (function waitForProcess(delay) {
          $timeout(function () {
            if (!processing) {
              if (!multiple && files.length > 1) {
                i = 0;
                while (files[i].type === 'directory') i++;
                files = [files[i]];
              }
              callback(files, rejFiles);
            } else {
              if (delays++ * 10 < 20 * 1000) {
                waitForProcess(10);
              }
            }
          }, delay || 0);
        })();
      }
    }
  }

  function dropAvailable() {
    var div = document.createElement('div');
    return ('draggable' in div) && ('ondrop' in div);
  }

})();

(function () {

  function fileToSrc(Upload, scope, $parse, attr, name, defaultName, callback) {
      if (defaultName) {
        callback($parse(defaultName)(scope));
      }
      scope.$watch(name, function (file) {
        if (!angular.isString(file)) {
          if (window.FileReader && ngFileUpload.validate(scope, $parse, attr, file, null)) {
            Upload.dataUrl(file, function (url) {
              if (callback) {
                callback(url);
              } else {
                file.dataUrl = url || $parse(defaultName)(scope);
              }
            }, $parse(attr.ngfNoObjectUrl)(scope));
          }
        } else {
          callback(file);
        }
      });
  }

  /** @namespace attr.ngfSrc */
  /** @namespace attr.ngfDefaultSrc */
  /** @namespace attr.ngfNoObjectUrl */
  ngFileUpload.directive('ngfSrc', ['$parse', 'Upload', function ($parse, Upload) {
    return {
      restrict: 'AE',
      link: function (scope, elem, attr) {
        fileToSrc(Upload, scope, $parse, attr, attr.ngfSrc, attr.ngfDefaultSrc, function (url) {
          elem.attr('src', url);
        });
      }
    };
  }]);

  /** @namespace attr.ngfBackground */
  /** @namespace attr.ngfDefaultBackground */
  /** @namespace attr.ngfNoObjectUrl */
  ngFileUpload.directive('ngfBackground', ['$parse', 'Upload', function ($parse, Upload) {
    return {
      restrict: 'AE',
      link: function (scope, elem, attr) {
        fileToSrc(Upload, scope, $parse, attr, attr.ngfBackground, attr.ngfDefaultBackground, function (url) {
          elem.css('background-image', 'url(' + url + ')');
        });
      }
    };
  }]);

  /** @namespace attr.ngfDataUrl */
  /** @namespace attr.ngfDefaultDataUrl */
  /** @namespace attr.ngfNoObjectUrl */
  ngFileUpload.directive('ngfDataUrl', ['$parse', 'Upload', function ($parse, Upload) {
    return {
      restrict: 'AE',
      link: function (scope, elem, attr) {

        fileToSrc(Upload, scope, $parse, attr, attr.ngfDataUrl, attr.ngfDefaultDataUrl);
      }
    };
  }]);
})();
