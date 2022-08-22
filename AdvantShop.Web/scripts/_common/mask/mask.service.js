export default function () {
    const service = this;
    const maskControlConfig = {};

    service.setMaskControlConfig = function (config) {
        maskControlConfig = Object.assign({}, config);
    };

    service.getMaskControlConfig = function () {
        return maskControlConfig;
    };
};

