export default function  iframeResponsiveService ($q, $window, $http) {
        var service = this,
            initializedYTList = [],
            initializedVimeoList = [],
            playerId = 0,
            regExpIdVideo = /^.*(youtu.be\/|v\/|e\/|u\/\w+\/|embed\/|v=)([^#\&\?]*).*/,
            regExpIframe = new RegExp('(?:<iframe[^>])'),
            regExpGetUrlFromSrc = new RegExp('(?:src=").*?(?=[\?"])'),
            urlRegex = new RegExp('(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})'),
            loadedYouTubeIframeAPI = false,
            loadedVimeoIframeAPI = false,
            activeItem;

        service.checkInitYouTubeIframeAPI = function () {
            return loadedYouTubeIframeAPI;
        };

        service.checkInitVimeoIframeAPI = function () {
            return loadedVimeoIframeAPI;
        };

        service.addYouTubeIframeAPI = function () {
            var tag = document.createElement('script');
            tag.src = "https://www.youtube.com/iframe_api";
            var firstScriptTag = document.getElementsByTagName('script')[0];
            firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
        };

        service.addVimeoIframeAPI = function () {
            var defer = $q.defer();
            initializedVimeoList.push(defer);
            var tag = document.createElement('script');
            tag.src = 'https://player.vimeo.com/api/player.js';
            tag.onload = function () {
                initializedVimeoList.forEach(function (defer) {
                    defer.resolve();
                });
                loadedVimeoIframeAPI = true;
            };
            var firstScriptTag = document.getElementsByTagName('script')[0];
            firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
            return defer.promise;
        };

        service.addOnYouTubeIframeAPIReady = function () {
            window.onYouTubeIframeAPIReady = function () {
                initializedYTList.forEach(function (defer) {
                    defer.resolve();
                });
                loadedYouTubeIframeAPI = true;
            };
            var defer = $q.defer();
            initializedYTList.push(defer);
            if (!service.checkInitYouTubeIframeAPI()) {
                service.addYouTubeIframeAPI();
            }
            return defer.promise;
        };

        service.getPlayerId = function () {
            return 'player' + (playerId += 1);
        };

        service.getVideoIdFromYouTube = function(url) {
            return url.match(regExpIdVideo)[2];
        };

        service.getVideoIdFromVimeo = function (url) {
            return url.split('vimeo.com/')[url.split('vimeo.com').length - 1];
        };

        service.getYTPlayerAPI = function (elId, videoId, callbacks, autoplay, loop) {
            return new YT.Player(elId, {
                videoId: videoId,
                host: 'https://www.youtube.com',
                playerVars: {
                    'rel': 0,
                    'enablejsapi': 1,
                    'modestbranding': 1,
                    'showinfo': 0,
                    'iv_load_policy': 3,
                    'origin': location.origin.toString(),
                    'autoplay': autoplay ? 1 : 0,
                    'controls': loop ? 0 : 1,
                    'loop': loop ? 1 : 0,
                    'playlist': videoId,
                    'mute': loop ? 1 : 0
                },
                events: callbacks
            });
        };

        service.getVimeoPlayerAPI = function (elId, videoId, autoplay, loop) {
            return new Vimeo.Player(elId, {
                id: videoId,
                autoplay: autoplay != null ? autoplay : false,
                muted: autoplay != null ? autoplay : false,
                loop: loop === true,
            });
        };


        service.run = function (obj, type) {
            if (activeItem != null && activeItem.obj !== obj && activeItem.obj.player != null) {
                if (activeItem.type === 'youtube') {
                    activeItem.obj.player.pauseVideo();
                } else if (activeItem.type === 'vimeo') {
                    activeItem.obj.player.pause();
                }
            }

            activeItem = {
                obj: obj,
                type: type
            };
        };

        service.checkUrlFromIframe = function (url) {
            return url.match(regExpIframe);
        };

        service.getSrc = function (url) {
            if (service.checkUrlFromIframe(url)) {
                return url.match(regExpGetUrlFromSrc)[0].match(urlRegex)[0];
            } 
            return url;
        };

        service.isPlayerCode = function (url) {
            return url.match(urlRegex) == null;
        };

        service.getYouTubeCode = function (link, autoplay, videoId, loop) {
            link = link.indexOf('https://') === -1 ? 'https://' + link : link;
            return link.replace('youtu.be', 'youtube.com/embed/').replace('watch?v=', 'embed/').split('&')[0] + '?rel=0&enablejsapi=1&modestbranding=1&showinfo=0&iv_load_policy=3' + (autoplay || loop ? '&autoplay=1&mute=1' : '') + (loop ? '&loop=1&controls=0&wmode=transparent&playlist=' + videoId + '' : '') + '&origin=' + location.origin;
        };

        service.getVimeoCode = function (link, autoplay, loop) {
            return 'https://player.vimeo.com/video' + link.split('vimeo.com')[link.split('vimeo.com').length - 1] + '?title=0&amp;byline=0&amp;portrait=0' + (autoplay ? '&autoplay=1&muted=1' : '') + (loop ? '&loop=1' : '');
        };

        service.getYTCover = function (YTVideoId) {
            return 'https://i.ytimg.com/vi/' + YTVideoId + '/maxresdefault.jpg';
        };

        service.getVimeoCover = function (vimeoId) {
            return $http.get('https://vimeo.com/api/oembed.json?url=https%3A//vimeo.com/' + vimeoId, {
                format: 'json',
                width: '1280'
            }).then(function (response) {
                return response;
            }).catch(function (error) {
                console.error(error);
            });

        };

    };

    iframeResponsiveService.$inject = ['$q', '$window', '$http'];


