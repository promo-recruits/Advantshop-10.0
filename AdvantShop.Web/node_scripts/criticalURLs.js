let list = new Map();
/**
 * ключ - название бандла
 * значение - строка или массив строк url (в случае вовзрата статус кода 404 берется следующий url)
 */
list.set('brand', 'brand');
list.set('cart', 'cart');
list.set('catalog', ['categories/kategoriya-1', 'categories/platia']);
list.set('catalogsearch', 'search?q=платье');
list.set('compare', 'compare');
list.set('feedback', 'feedback');
list.set('giftcertificate', 'giftcertificate');
list.set('forgotpassword', 'forgotpassword');
list.set('home', '');
list.set('login', 'login');
list.set('managers', 'managers');
list.set('myaccount', { url: 'myaccount', admin: true });
list.set('news', 'news');
list.set('registration', 'registration');
list.set('product', ['products/vash-tovar-5', 'products/dress1']);
list.set('productlist', ['productlist/best', 'productlist/new', 'productlist/sale', 'productlist/newarrivals']);
list.set('staticpage', 'pages/contacts');
list.set('wishlist', 'wishlist');

exports.list = list;