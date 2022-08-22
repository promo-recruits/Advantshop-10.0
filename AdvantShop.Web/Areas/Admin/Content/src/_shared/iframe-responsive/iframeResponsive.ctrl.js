﻿; (function (ng) {
    'use strict';

    var IframeResponsiveCtrl = function ($sce, iframeResponsiveService, $scope, $timeout) {
        var ctrl = this,
            urlRegex = new RegExp('(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})'),
            stateChangeFlag = true;

        ctrl.showContent = function () {
            ctrl.isShowContent = true;
            if (ctrl.deviceMobile && ctrl.asBackground) {

                if (ctrl.useVimeo) {
                    var vimeoVideoId = iframeResponsiveService.getVideoIdFromVimeo(ctrl.src);
                    iframeResponsiveService.getVimeoCover(vimeoVideoId).then(function (response) {
                        if (response.data != null) {
                            ctrl.coverVideoPath = response.data.thumbnail_url;
                        }
                    });
                } else if (ctrl.useYouTube) {
                    var YTVideoId = iframeResponsiveService.getVideoIdFromYouTube(ctrl.src);
                    ctrl.coverVideoPath = iframeResponsiveService.getYTCover(YTVideoId);
                }
            } else {
                if (ctrl.isPlayerCode) {
                    ctrl.playerCode = ctrl.src;
                } else {
                    ctrl.src = iframeResponsiveService.getSrc(ctrl.src);
                }

                if (ctrl.inModal === true) {
                    ctrl.pasteVideoForModal(ctrl.src);
                } else {
                    //$timeout(function () {
                        ctrl.pasteVideo(ctrl.src, ctrl.autoplay, ctrl.loop);
                    //}, 0);
                }
            }
        };

        ctrl.pasteVideoForModal = function (src) {
            ctrl.stopOthersVideo();
            ctrl.showVideo();
            ctrl.hideCover();
            if (ctrl.useYouTube) {
                src = iframeResponsiveService.getYouTubeCode(src, true)
            }
            if (ctrl.useVimeo) {
                src = iframeResponsiveService.getVimeoCode(src, true)
            }

            ctrl.iframeSrc = $sce.trustAsResourceUrl(src);  
        };

        ctrl.onPlayerReady = function (event) {
            if (ctrl.autoplay) {
                ctrl.player.mute();
                ctrl.player.playVideo();
            }

            if (stateChangeFlag) {
                stateChangeFlag = false;
            }
        };

        ctrl.onPlayerStateChange = function (event) {
            ctrl.videoLoaded = true;
            if (event.data === -1) {
                ctrl.muteOn = true; // autoplay

                $timeout(function () {
                    ctrl.hideCover();
                }, 100);
            } else if (event.data === 1) {
                if (!ctrl.disabledStop) {
                    iframeResponsiveService.run(ctrl, 'youtube');
                }

            }
        };

        ctrl.showVideo = function () {
            ctrl.visibleVideo = true;
        };

        ctrl.hideVideo = function () {
            ctrl.visibleVideo = false;
        };

        ctrl.showCover = function () {
            ctrl.visibleCover = true;
        };

        ctrl.hideCover = function () {
            ctrl.visibleCover = false;
        };

        ctrl.stopOthersVideo = function () {
            if (!ctrl.disabledStop) {
                iframeResponsiveService.run(ctrl, 'vimeo');
                iframeResponsiveService.run(ctrl, 'youtube');
            }
        };

        ctrl.pasteYTIframeSrc = function (src, playerId, autoplay, loop) {

            var YTVideoId = iframeResponsiveService.getVideoIdFromYouTube(src);
            ctrl.coverVideoPath = iframeResponsiveService.getYTCover(YTVideoId);
            $timeout(function () {

                if (!iframeResponsiveService.checkInitYouTubeIframeAPI()) {
                    iframeResponsiveService.addOnYouTubeIframeAPIReady()
                        .then(function () {
                            ctrl.player = iframeResponsiveService.getYTPlayerAPI(playerId, YTVideoId, { 'onReady': ctrl.onPlayerReady, 'onStateChange': ctrl.onPlayerStateChange }, autoplay, loop);
                        })
                        .catch(function (error) {
                            console.error(error);
                        });
                } else {
                    ctrl.player = iframeResponsiveService.getYTPlayerAPI(ctrl.playerId, YTVideoId, { 'onReady': ctrl.onPlayerReady, 'onStateChange': ctrl.onPlayerStateChange }, autoplay);
                }

            });
            var YTCode = iframeResponsiveService.getYouTubeCode(src, autoplay, YTVideoId, loop);
            ctrl.iframeSrc = $sce.trustAsResourceUrl(YTCode);
        };

        ctrl.pasteVimeoIframeSrc = function (src, playerId, autoplay, loop) {
            var vimeoVideoId = iframeResponsiveService.getVideoIdFromVimeo(src);

            iframeResponsiveService.getVimeoCover(vimeoVideoId).then(function (response) {
                if (response.data != null) {
                    ctrl.coverVideoPath = response.data.thumbnail_url;
                }
            });
            $timeout(function () {
                if (!iframeResponsiveService.checkInitVimeoIframeAPI()) {
                    iframeResponsiveService.addVimeoIframeAPI().then(function () {
                        ctrl.player = iframeResponsiveService.getVimeoPlayerAPI(playerId, vimeoVideoId, autoplay, loop);
                        ctrl.player.on('play', function () {
                            ctrl.stopOthersVideo();
                            iframeResponsiveService.run(ctrl, 'vimeo');
                            ctrl.hideCover();
                            $scope.$digest();
                        });
                    }).catch(function (error) {
                        console.error(error);
                    });
                } else {
                    ctrl.player = iframeResponsiveService.getVimeoPlayerAPI(playerId, vimeoVideoId, autoplay, loop);
                }
            });
        };

        ctrl.pasteVideo = function (src, autoplay, loop) {
            ctrl.playerId = iframeResponsiveService.getPlayerId();
            if (ctrl.useYouTube) {
                ctrl.pasteYTIframeSrc(src, ctrl.playerId, autoplay, loop);
            }
            if (ctrl.useVimeo) {
                ctrl.pasteVimeoIframeSrc(src, ctrl.playerId, autoplay, loop);
            }
        };
    };

    ng.module('iframeResponsive')
        .controller('IframeResponsiveCtrl', IframeResponsiveCtrl);

    IframeResponsiveCtrl.$inject = ['$sce', 'iframeResponsiveService', '$scope', '$timeout'];

})(window.angular);
