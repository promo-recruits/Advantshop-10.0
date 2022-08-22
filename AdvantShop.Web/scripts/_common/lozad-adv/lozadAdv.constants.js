const lozadAdvConstants = {
    rootMargin: '0px',
    threshold: 0,
    afterWindowLoaded: true,
    load: function load(element) {
        if (element.dataset.src) {
            element.src = element.dataset.src;
        }
        if (element.dataset.srcset) {
            element.srcset = element.dataset.srcset;
        }
        if (element.dataset.backgroundImage) {
            element.style.backgroundImage = 'url(' + element.dataset.backgroundImage + ')';
        }
    }
}

export default lozadAdvConstants;