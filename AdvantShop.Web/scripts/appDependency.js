class AppDependency {

    constructor() {
        if (!window.___appDependency) {
            window.___appDependency = this;
            window.___appDependencyList = [];
        }

        return window.___appDependency;
    }

    addItem(moduleName) {
        window.___appDependencyList.push(moduleName);
    }

    addList(moduleNameList) {
        window.___appDependencyList = window.___appDependencyList.concat(moduleNameList);
    }

    get() {
        return window.___appDependencyList;
    }
}

export default new AppDependency();