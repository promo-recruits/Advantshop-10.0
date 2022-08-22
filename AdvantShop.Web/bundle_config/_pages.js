const PagesStorage = require('../node_scripts/pagesStorage.js');

let obj = new PagesStorage();

obj.addItem('brand', __dirname + '/brand.js');
obj.addItem('billing', __dirname + '/billing.js');
obj.addItem('cart', __dirname + '/cart.js');
obj.addItem('catalog', __dirname + '/catalog.js');
obj.addItem('catalogSearch', __dirname + '/catalogSearch.js');
obj.addItem('checkout', __dirname + '/checkout.js');
obj.addItem('checkoutSuccess', __dirname + '/checkoutSuccess.js');
obj.addItem('compare', __dirname + '/compare.js');
obj.addItem('error', __dirname + '/error.js');
obj.addItem('feedback', __dirname + '/feedback.js');
obj.addItem('giftcertificate', __dirname + '/giftcertificate.js');
obj.addItem('giftcertificatePrint', __dirname + '/giftcertificatePrint.js');
obj.addItem('forgotPassword', __dirname + '/forgotPassword.js');
obj.addItem('home', __dirname + '/home.js');
obj.addItem('login', __dirname + '/login.js');
obj.addItem('managers', __dirname + '/managers.js');
obj.addItem('myaccount', __dirname + '/myaccount.js');
obj.addItem('myaccountFunnel', __dirname + '/myaccountFunnel.js');
obj.addItem('news', __dirname + '/news.js');
obj.addItem('preorder', __dirname + '/preorder.js');
obj.addItem('printorder', __dirname + '/printorder.js');
obj.addItem('product', __dirname + '/product.js');
obj.addItem('productList', __dirname + '/productList.js');
obj.addItem('registration', __dirname + '/registration.js');
obj.addItem('staticPage', __dirname + '/staticPage.js');
obj.addItem('wishlist', __dirname + '/wishlist.js');
obj.addItem('bonusPage', __dirname + '/bonusPage.js');

//отдельные бандлы, которые подключаются через ocLazyLoad
obj.addItem('inplaceMin', __dirname + '/inplaceMin.js');
obj.addItem('inplaceMax', __dirname + '/inplaceMax.js');

obj.addItem('inplaceFunnelMin', __dirname + '/inplaceFunnelMin.js');
obj.addItem('inplaceFunnelMax', __dirname + '/inplaceFunnelMax.js');

obj.addItem('mobileOverlap', __dirname + '/mobileOverlap.js');
obj.addItem('currency', __dirname + '/currency.js');
obj.addItem('cookiesPolicy', __dirname + '/cookiesPolicy.js');
obj.addItem('demo', __dirname + '/demo.js');
obj.addItem('builder', __dirname + '/builder.js');
obj.addItem('logogenerator', __dirname + '/logogenerator.js');
obj.addItem('logogeneratorFunnel', __dirname + '/logogeneratorFunnel.js');
obj.addItem('telephony', __dirname + '/telephony.js');

obj.addItem('common', __dirname + '/common.js');
obj.addItem('head', __dirname + '/head.js');

module.exports = obj;