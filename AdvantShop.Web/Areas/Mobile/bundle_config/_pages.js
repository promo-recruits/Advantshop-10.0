const PagesStorage = require('../../../node_scripts/pagesStorage.js');

let pages = new PagesStorage();

pages.addItem('brand', __dirname + '/brand.js');
pages.addItem('billing', __dirname + '/billing.js');
pages.addItem('cart', __dirname + '/cart.js');
pages.addItem('catalog', __dirname + '/catalog.js');
pages.addItem('checkout', __dirname + '/checkout.js');
pages.addItem('checkoutSuccess', __dirname + '/checkoutSuccess.js');
pages.addItem('compare', __dirname + '/compare.js');
pages.addItem('error', __dirname + '/error.js');
pages.addItem('feedback', __dirname + '/feedback.js');
pages.addItem('giftcertificate', __dirname + '/giftcertificate.js');
pages.addItem('giftcertificatePrint', __dirname + '/giftcertificatePrint.js');
pages.addItem('forgotPassword', __dirname + '/forgotPassword.js');
pages.addItem('home', __dirname + '/home.js');
pages.addItem('login', __dirname + '/login.js');
pages.addItem('managers', __dirname + '/managers.js');
pages.addItem('myaccount', __dirname + '/myaccount.js');
pages.addItem('news', __dirname + '/news.js');
pages.addItem('preorder', __dirname + '/preorder.js');
pages.addItem('product', __dirname + '/product.js');
pages.addItem('productList', __dirname + '/productList.js');
pages.addItem('registration', __dirname + '/registration.js');
pages.addItem('staticPage', __dirname + '/staticPage.js');
pages.addItem('wishlist', __dirname + '/wishlist.js');
pages.addItem('bonusPage', __dirname + '/bonusPage.js');

//для мобилки
pages.addItem('catalogSearch', __dirname + '/catalogSearch.js');
pages.addItem('checkoutMobile', __dirname + '/checkoutMobile.js');

//нужен для модулей, которые выводят свои вьюшки, к примеру, модуль "Блог"
pages.addItem('common', __dirname + '/common.js');
pages.addItem('head', __dirname + '/head.js');

module.exports = pages;