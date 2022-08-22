self.onmessage = function (event) {
    var data = event.data;
    self.importScripts(data.scriptSrc);
    var result = self.hljs.highlightAuto(data.code);
    self.postMessage(result.value);
    self.close();
};