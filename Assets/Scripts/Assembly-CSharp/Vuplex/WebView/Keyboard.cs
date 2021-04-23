﻿using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	public class Keyboard : MonoBehaviour
	{
		
		
		
		public event EventHandler<EventArgs<string>> InputReceived;

		
		
		
		public event EventHandler Initialized;

		
		
		
		public WebViewPrefab WebViewPrefab { get; private set; }

		
		public static Keyboard Instantiate()
		{
			return Keyboard.Instantiate(0.5f, 0.125f);
		}

		
		public static Keyboard Instantiate(float width, float height)
		{
			Keyboard keyboard = new GameObject("Keyboard").AddComponent<Keyboard>();
			keyboard.Init(width, height);
			return keyboard;
		}

		
		public void Init(float width, float height)
		{
			this.WebViewPrefab = WebViewPrefab.Instantiate(width, height, new WebViewOptions
			{
				clickWithoutStealingFocus = true,
				disableVideo = true,
				preferredPlugins = new WebPluginType[1]
			});
			this.WebViewPrefab.transform.parent = base.transform;
			this.WebViewPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
			this.WebViewPrefab.Initialized += delegate(object sender, EventArgs e)
			{
				WebPluginType pluginType = this.WebViewPrefab.WebView.PluginType;
				if (pluginType == WebPluginType.AndroidGecko)
				{
					this.WebViewPrefab.HoveringEnabled = false;
				}
				this.WebViewPrefab.WebView.MessageEmitted += this.WebView_MessageEmitted;
				if (pluginType == WebPluginType.AndroidGecko || pluginType == WebPluginType.UniversalWindowsPlatform)
				{
					this.WebViewPrefab.SetCutoutRect(new Rect(0f, 0f, 1f, 1f));
				}
				this.WebViewPrefab.WebView.LoadHtml("<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"transparent\" content=\"true\"><meta name=\"viewport\" content=\"viewport-fit=cover,width=device-width,initial-scale=1,minimum-scale=1,maximum-scale=1,user-scalable=0\"><meta name=\"theme-color\" content=\"#000000\"><link rel=\"manifest\" href=\"./manifest.json\"><link rel=\"shortcut icon\" href=\"./favicon.ico\"><title>Unity Keyboard</title><style>html{width:100vw;height:100vh}body{margin:0;padding:0;font-family:Helvetica,Ariel,sans-serif;width:100%;height:100%;font-size:20px}@supports (-moz-appearance:none){body{background-color:#000}}#root{width:100%;height:100%}.key-component{display:-webkit-flex;display:flex;-webkit-justify-content:center;justify-content:center;-webkit-align-items:center;align-items:center;border-radius:.5rem}.key-component>img,.key-component div,.key-component span{pointer-events:none}.key-component.hover,.key-component:hover{background-color:#485a63}.key-component.key-down{background-color:#a2b3bc}.key-component.key-up{-webkit-animation:highlight 1s;animation:highlight 1s}@-webkit-keyframes highlight{0%{background-color:#a2b3bc}to{background-color:auto}}@keyframes highlight{0%{background-color:#a2b3bc}to{background-color:auto}}.center-board{background-color:#283237;color:#fff;height:100%;width:100%;border-radius:.6rem;display:-webkit-flex;display:flex;-webkit-justify-content:center;justify-content:center;-webkit-align-items:center;align-items:center}.center-board .key-area{display:-webkit-flex;display:flex;-webkit-flex-direction:column;flex-direction:column;width:95%;height:90%}.center-board .key-area .key-row{display:-webkit-flex;display:flex;-webkit-justify-content:space-around;justify-content:space-around;-webkit-flex:1 1 auto;flex:1 1 auto}.center-board .key-area .key-row .key{-webkit-flex:2 0 auto;flex:2 0 auto}.center-board .key-area .key-row .key-offset-margin{-webkit-flex:1 0 auto;flex:1 0 auto}.center-board .key-area .key-row .spacebar-container{-webkit-flex:12 0 auto;flex:12 0 auto;display:-webkit-flex;display:flex;-webkit-justify-content:center;justify-content:center;-webkit-align-items:center;align-items:center}.center-board .key-area .key-row .spacebar-container .spacebar{width:98%;height:80%;background-color:#3d4d55;border-radius:.6rem;color:#a6a6a6}.center-board .key-area .key-row .spacebar-container .spacebar.hover,.center-board .key-area .key-row .spacebar-container .spacebar:hover{background-color:#485a63}.center-board .key-area .key-row .spacebar-container .spacebar:active{background-color:#688390}.shift-key-icon{display:-webkit-flex;display:flex;-webkit-flex-direction:column;flex-direction:column;justify-conten:center;-webkit-align-items:center;align-items:center}.shift-key-icon .shift-arrow-triangle{width:0;height:0;border-left:.45em solid transparent;border-bottom:.45em solid #fff;border-right:.45em solid transparent}.shift-key-icon .shift-arrow-triangle.shift-arrow-colored{border-bottom-color:#42c695}.shift-key-icon .regular-arrow-line{height:.6em;width:.35em;background-color:#fff}.shift-key-icon .regular-arrow-line.shift-arrow-colored{background-color:#42c695}.shift-key-icon .caps-lock-arrow-line .caps-lock-line-top{height:.3em;width:.35em;background-color:#42c695}.shift-key-icon .caps-lock-arrow-line .caps-lock-line-middle{height:.2em}.shift-key-icon .caps-lock-arrow-line .caps-lock-line-bottom{height:.1em;width:.35em;background-color:#42c695}.num-pad{width:100%;height:100%;border-radius:.6rem;background-color:#283237;color:#fff;-webkit-justify-content:center;justify-content:center;-webkit-align-items:center;align-items:center}.num-pad,.num-pad .key-area{display:-webkit-flex;display:flex}.num-pad .key-area{-webkit-flex-direction:column;flex-direction:column;width:85%;height:90%}.num-pad .key-area .key-row{display:-webkit-flex;display:flex;-webkit-justify-content:space-around;justify-content:space-around;-webkit-flex:1 1 auto;flex:1 1 auto}.num-pad .key-area .key-row .key{-webkit-flex:1 0 auto;flex:1 0 auto}.right-pad{width:100%;height:100%;background-color:#283237;border-radius:.6rem;color:#fff;-webkit-justify-content:center;justify-content:center;-webkit-align-items:center;align-items:center}.right-pad,.right-pad .key-area{display:-webkit-flex;display:flex}.right-pad .key-area{width:90%;height:75%;-webkit-justify-content:space-between;justify-content:space-between}.right-pad .right-key-area{-webkit-flex:0 0 100%;flex:0 0 100%;display:-webkit-flex;display:flex;-webkit-align-items:center;align-items:center;-webkit-justify-content:center;justify-content:center}.right-pad .right-key-area .arrow-pad{width:90%;height:3.75em;display:-webkit-flex;display:flex;-webkit-flex-direction:column;flex-direction:column;font-size:1.25em}.right-pad .right-key-area .arrow-pad .arrow-pad-row{-webkit-flex:1 1 auto;flex:1 1 auto;width:100%;display:-webkit-flex;display:flex;-webkit-justify-content:center;justify-content:center;-webkit-align-items:center;align-items:center}.right-pad .right-key-area .arrow-pad .arrow-pad-row.arrow-pad-middle{-webkit-justify-content:space-between;justify-content:space-between}.voice-button{width:100%;padding-top:100%;position:relative;border-radius:50%;background-color:#a6a6a6}.voice-button .voice-button-icon{position:absolute;top:0;right:0;bottom:0;left:0;display:-webkit-flex;display:flex;-webkit-justify-content:center;justify-content:center;-webkit-align-items:center;align-items:center;pointer-events:none}.voice-button .voice-button-icon>div{width:50%;height:50%}.voice-button .voice-button-icon>div img{width:100%;height:100%}.voice-button svg{position:absolute;top:-224px;left:-224px;-webkit-transform:scale(.05);transform:scale(.05);fill:rgba(0,0,0,.5);pointer-events:none}.voice-button.hover,.voice-button:hover{background-color:#56cca0}.voice-button.hover svg,.voice-button:hover svg{fill:#283237}@-webkit-keyframes recordingBackground{0%{background:#f30}50%{background:#ff8000}to{background:#f30}}@keyframes recordingBackground{0%{background:#f30}50%{background:#ff8000}to{background:#f30}}.voice-button.voice-button-active{-webkit-animation:recordingBackground 5s infinite;animation:recordingBackground 5s infinite}.voice-button.voice-button-active svg{fill:#fff}.keyboard{width:100%;height:100%;display:-webkit-flex;display:flex;-webkit-justify-content:space-between;justify-content:space-between}.keyboard .num-pad-container{-webkit-flex:1 1 15%;flex:1 1 15%}.keyboard .center-board-container{-webkit-flex:1 1 55%;flex:1 1 55%}.keyboard .right-pad-container{-webkit-flex:1 1 15%;flex:1 1 15%}.keyboard .board-margin{-webkit-flex:1 1 2.5%;flex:1 1 2.5%}.keyboard .enter-key-area{height:100%;-webkit-flex:1 1 10%;flex:1 1 10%;display:-webkit-flex;display:flex;-webkit-flex-direction:column;flex-direction:column;-webkit-justify-content:center;justify-content:center;-webkit-align-items:center;align-items:center}.keyboard .enter-key-area .backspace-icon{width:60%;border-radius:.5rem}.keyboard .enter-key-area .backspace-icon:hover{background-color:#dde4e7}.keyboard .enter-key-area .backspace-icon img{width:100%}.keyboard .enter-key-area .return-key-container{width:60%;padding-top:10px}.keyboard .enter-key-area .return-key-container .return-key-component{border-radius:50%;background-color:#09f}.keyboard .enter-key-area .return-key-container .return-key-component.hover,.keyboard .enter-key-area .return-key-container .return-key-component:hover{background-color:#008ae6}.keyboard .enter-key-area .return-key-container .return-key{width:100%;padding-top:100%;position:relative;border-radius:50%;pointer-events:none}.keyboard .enter-key-area .voice-button-container{width:60%;padding-top:15px}.keyboard .enter-key-area .return-key-text{position:absolute;top:0;left:0;bottom:0;right:0;display:-webkit-flex;display:flex;-webkit-justify-content:center;justify-content:center;-webkit-align-items:center;align-items:center;font-size:45px;font-weight:700}.keyboard .enter-key-area .return-key-text img{width:60%}/*# sourceMappingURL=main.a3550cfb.chunk.css.map */</style></head><body><noscript>You need to enable JavaScript to run this app.</noscript><div id=\"root\"></div><script>!function(l){function e(e){for(var r,t,n=e[0],o=e[1],u=e[2],f=0,i=[];f<n.length;f++)t=n[f],p[t]&&i.push(p[t][0]),p[t]=0;for(r in o)Object.prototype.hasOwnProperty.call(o,r)&&(l[r]=o[r]);for(s&&s(e);i.length;)i.shift()();return c.push.apply(c,u||[]),a()}function a(){for(var e,r=0;r<c.length;r++){for(var t=c[r],n=!0,o=1;o<t.length;o++){var u=t[o];0!==p[u]&&(n=!1)}n&&(c.splice(r--,1),e=f(f.s=t[0]))}return e}var t={},p={2:0},c=[];function f(e){if(t[e])return t[e].exports;var r=t[e]={i:e,l:!1,exports:{}};return l[e].call(r.exports,r,r.exports,f),r.l=!0,r.exports}f.m=l,f.c=t,f.d=function(e,r,t){f.o(e,r)||Object.defineProperty(e,r,{enumerable:!0,get:t})},f.r=function(e){\"undefined\"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:\"Module\"}),Object.defineProperty(e,\"__esModule\",{value:!0})},f.t=function(r,e){if(1&e&&(r=f(r)),8&e)return r;if(4&e&&\"object\"==typeof r&&r&&r.__esModule)return r;var t=Object.create(null);if(f.r(t),Object.defineProperty(t,\"default\",{enumerable:!0,value:r}),2&e&&\"string\"!=typeof r)for(var n in r)f.d(t,n,function(e){return r[e]}.bind(null,n));return t},f.n=function(e){var r=e&&e.__esModule?function(){return e.default}:function(){return e};return f.d(r,\"a\",r),r},f.o=function(e,r){return Object.prototype.hasOwnProperty.call(e,r)},f.p=\"\";var r=window.webpackJsonp=window.webpackJsonp||[],n=r.push.bind(r);r.push=e,r=r.slice();for(var o=0;o<r.length;o++)e(r[o]);var s=n;a()}([])</script><script>(window.webpackJsonp=window.webpackJsonp||[]).push([[1],[function(e,t,n){\"use strict\";e.exports=n(18)},function(e,t,n){\"use strict\";function r(e){return(r=Object.setPrototypeOf?Object.getPrototypeOf:function(e){return e.__proto__||Object.getPrototypeOf(e)})(e)}n.d(t,\"a\",function(){return r})},function(e,t,n){\"use strict\";function r(e,t){if(!(e instanceof t))throw new TypeError(\"Cannot call a class as a function\")}n.d(t,\"a\",function(){return r})},function(e,t,n){\"use strict\";function r(e,t){for(var n=0;n<t.length;n++){var r=t[n];r.enumerable=r.enumerable||!1,r.configurable=!0,\"value\"in r&&(r.writable=!0),Object.defineProperty(e,r.key,r)}}function i(e,t,n){return t&&r(e.prototype,t),n&&r(e,n),e}n.d(t,\"a\",function(){return i})},function(e,t,n){\"use strict\";function r(e){return(r=\"function\"===typeof Symbol&&\"symbol\"===typeof Symbol.iterator?function(e){return typeof e}:function(e){return e&&\"function\"===typeof Symbol&&e.constructor===Symbol&&e!==Symbol.prototype?\"symbol\":typeof e})(e)}function i(e){return(i=\"function\"===typeof Symbol&&\"symbol\"===r(Symbol.iterator)?function(e){return r(e)}:function(e){return e&&\"function\"===typeof Symbol&&e.constructor===Symbol&&e!==Symbol.prototype?\"symbol\":r(e)})(e)}var l=n(7);function o(e,t){return!t||\"object\"!==i(t)&&\"function\"!==typeof t?Object(l.a)(e):t}n.d(t,\"a\",function(){return o})},function(e,t,n){\"use strict\";function r(e,t){return(r=Object.setPrototypeOf||function(e,t){return e.__proto__=t,e})(e,t)}function i(e,t){if(\"function\"!==typeof t&&null!==t)throw new TypeError(\"Super expression must either be null or a function\");e.prototype=Object.create(t&&t.prototype,{constructor:{value:e,writable:!0,configurable:!0}}),t&&r(e,t)}n.d(t,\"a\",function(){return i})},function(e,t,n){\"use strict\";var r=n(1);function i(e,t,n){return(i=\"undefined\"!==typeof Reflect&&Reflect.get?Reflect.get:function(e,t,n){var i=function(e,t){for(;!Object.prototype.hasOwnProperty.call(e,t)&&null!==(e=Object(r.a)(e)););return e}(e,t);if(i){var l=Object.getOwnPropertyDescriptor(i,t);return l.get?l.get.call(n):l.value}})(e,t,n||e)}n.d(t,\"a\",function(){return i})},function(e,t,n){\"use strict\";function r(e){if(void 0===e)throw new ReferenceError(\"this hasn't been initialised - super() hasn't been called\");return e}n.d(t,\"a\",function(){return r})},function(e,t,n){\"use strict\";function r(e){return function(e){if(Array.isArray(e)){for(var t=0,n=new Array(e.length);t<e.length;t++)n[t]=e[t];return n}}(e)||function(e){if(Symbol.iterator in Object(e)||\"[object Arguments]\"===Object.prototype.toString.call(e))return Array.from(e)}(e)||function(){throw new TypeError(\"Invalid attempt to spread non-iterable instance\")}()}n.d(t,\"a\",function(){return r})},function(e,t,n){\"use strict\";var r,i=\"object\"===typeof Reflect?Reflect:null,l=i&&\"function\"===typeof i.apply?i.apply:function(e,t,n){return Function.prototype.apply.call(e,t,n)};r=i&&\"function\"===typeof i.ownKeys?i.ownKeys:Object.getOwnPropertySymbols?function(e){return Object.getOwnPropertyNames(e).concat(Object.getOwnPropertySymbols(e))}:function(e){return Object.getOwnPropertyNames(e)};var o=Number.isNaN||function(e){return e!==e};function a(){a.init.call(this)}e.exports=a,a.EventEmitter=a,a.prototype._events=void 0,a.prototype._eventsCount=0,a.prototype._maxListeners=void 0;var u=10;function c(e){return void 0===e._maxListeners?a.defaultMaxListeners:e._maxListeners}function s(e,t,n,r){var i,l,o,a;if(\"function\"!==typeof n)throw new TypeError('The \"listener\" argument must be of type Function. Received type '+typeof n);if(void 0===(l=e._events)?(l=e._events=Object.create(null),e._eventsCount=0):(void 0!==l.newListener&&(e.emit(\"newListener\",t,n.listener?n.listener:n),l=e._events),o=l[t]),void 0===o)o=l[t]=n,++e._eventsCount;else if(\"function\"===typeof o?o=l[t]=r?[n,o]:[o,n]:r?o.unshift(n):o.push(n),(i=c(e))>0&&o.length>i&&!o.warned){o.warned=!0;var u=new Error(\"Possible EventEmitter memory leak detected. \"+o.length+\" \"+String(t)+\" listeners added. Use emitter.setMaxListeners() to increase limit\");u.name=\"MaxListenersExceededWarning\",u.emitter=e,u.type=t,u.count=o.length,a=u,console&&console.warn&&console.warn(a)}return e}function f(e,t,n){var r={fired:!1,wrapFn:void 0,target:e,type:t,listener:n},i=function(){for(var e=[],t=0;t<arguments.length;t++)e.push(arguments[t]);this.fired||(this.target.removeListener(this.type,this.wrapFn),this.fired=!0,l(this.listener,this.target,e))}.bind(r);return i.listener=n,r.wrapFn=i,i}function d(e,t,n){var r=e._events;if(void 0===r)return[];var i=r[t];return void 0===i?[]:\"function\"===typeof i?n?[i.listener||i]:[i]:n?function(e){for(var t=new Array(e.length),n=0;n<t.length;++n)t[n]=e[n].listener||e[n];return t}(i):m(i,i.length)}function p(e){var t=this._events;if(void 0!==t){var n=t[e];if(\"function\"===typeof n)return 1;if(void 0!==n)return n.length}return 0}function m(e,t){for(var n=new Array(t),r=0;r<t;++r)n[r]=e[r];return n}Object.defineProperty(a,\"defaultMaxListeners\",{enumerable:!0,get:function(){return u},set:function(e){if(\"number\"!==typeof e||e<0||o(e))throw new RangeError('The value of \"defaultMaxListeners\" is out of range. It must be a non-negative number. Received '+e+\".\");u=e}}),a.init=function(){void 0!==this._events&&this._events!==Object.getPrototypeOf(this)._events||(this._events=Object.create(null),this._eventsCount=0),this._maxListeners=this._maxListeners||void 0},a.prototype.setMaxListeners=function(e){if(\"number\"!==typeof e||e<0||o(e))throw new RangeError('The value of \"n\" is out of range. It must be a non-negative number. Received '+e+\".\");return this._maxListeners=e,this},a.prototype.getMaxListeners=function(){return c(this)},a.prototype.emit=function(e){for(var t=[],n=1;n<arguments.length;n++)t.push(arguments[n]);var r=\"error\"===e,i=this._events;if(void 0!==i)r=r&&void 0===i.error;else if(!r)return!1;if(r){var o;if(t.length>0&&(o=t[0]),o instanceof Error)throw o;var a=new Error(\"Unhandled error.\"+(o?\" (\"+o.message+\")\":\"\"));throw a.context=o,a}var u=i[e];if(void 0===u)return!1;if(\"function\"===typeof u)l(u,this,t);else{var c=u.length,s=m(u,c);for(n=0;n<c;++n)l(s[n],this,t)}return!0},a.prototype.addListener=function(e,t){return s(this,e,t,!1)},a.prototype.on=a.prototype.addListener,a.prototype.prependListener=function(e,t){return s(this,e,t,!0)},a.prototype.once=function(e,t){if(\"function\"!==typeof t)throw new TypeError('The \"listener\" argument must be of type Function. Received type '+typeof t);return this.on(e,f(this,e,t)),this},a.prototype.prependOnceListener=function(e,t){if(\"function\"!==typeof t)throw new TypeError('The \"listener\" argument must be of type Function. Received type '+typeof t);return this.prependListener(e,f(this,e,t)),this},a.prototype.removeListener=function(e,t){var n,r,i,l,o;if(\"function\"!==typeof t)throw new TypeError('The \"listener\" argument must be of type Function. Received type '+typeof t);if(void 0===(r=this._events))return this;if(void 0===(n=r[e]))return this;if(n===t||n.listener===t)0===--this._eventsCount?this._events=Object.create(null):(delete r[e],r.removeListener&&this.emit(\"removeListener\",e,n.listener||t));else if(\"function\"!==typeof n){for(i=-1,l=n.length-1;l>=0;l--)if(n[l]===t||n[l].listener===t){o=n[l].listener,i=l;break}if(i<0)return this;0===i?n.shift():function(e,t){for(;t+1<e.length;t++)e[t]=e[t+1];e.pop()}(n,i),1===n.length&&(r[e]=n[0]),void 0!==r.removeListener&&this.emit(\"removeListener\",e,o||t)}return this},a.prototype.off=a.prototype.removeListener,a.prototype.removeAllListeners=function(e){var t,n,r;if(void 0===(n=this._events))return this;if(void 0===n.removeListener)return 0===arguments.length?(this._events=Object.create(null),this._eventsCount=0):void 0!==n[e]&&(0===--this._eventsCount?this._events=Object.create(null):delete n[e]),this;if(0===arguments.length){var i,l=Object.keys(n);for(r=0;r<l.length;++r)\"removeListener\"!==(i=l[r])&&this.removeAllListeners(i);return this.removeAllListeners(\"removeListener\"),this._events=Object.create(null),this._eventsCount=0,this}if(\"function\"===typeof(t=n[e]))this.removeListener(e,t);else if(void 0!==t)for(r=t.length-1;r>=0;r--)this.removeListener(e,t[r]);return this},a.prototype.listeners=function(e){return d(this,e,!0)},a.prototype.rawListeners=function(e){return d(this,e,!1)},a.listenerCount=function(e,t){return\"function\"===typeof e.listenerCount?e.listenerCount(t):p.call(e,t)},a.prototype.listenerCount=p,a.prototype.eventNames=function(){return this._eventsCount>0?r(this._events):[]}},function(e,t,n){\"use strict\";var r=Object.getOwnPropertySymbols,i=Object.prototype.hasOwnProperty,l=Object.prototype.propertyIsEnumerable;e.exports=function(){try{if(!Object.assign)return!1;var e=new String(\"abc\");if(e[5]=\"de\",\"5\"===Object.getOwnPropertyNames(e)[0])return!1;for(var t={},n=0;n<10;n++)t[\"_\"+String.fromCharCode(n)]=n;if(\"0123456789\"!==Object.getOwnPropertyNames(t).map(function(e){return t[e]}).join(\"\"))return!1;var r={};return\"abcdefghijklmnopqrst\".split(\"\").forEach(function(e){r[e]=e}),\"abcdefghijklmnopqrst\"===Object.keys(Object.assign({},r)).join(\"\")}catch(i){return!1}}()?Object.assign:function(e,t){for(var n,o,a=function(e){if(null===e||void 0===e)throw new TypeError(\"Object.assign cannot be called with null or undefined\");return Object(e)}(e),u=1;u<arguments.length;u++){for(var c in n=Object(arguments[u]))i.call(n,c)&&(a[c]=n[c]);if(r){o=r(n);for(var s=0;s<o.length;s++)l.call(n,o[s])&&(a[o[s]]=n[o[s]])}}return a}},function(e,t,n){\"use strict\";!function e(){if(\"undefined\"!==typeof __REACT_DEVTOOLS_GLOBAL_HOOK__&&\"function\"===typeof __REACT_DEVTOOLS_GLOBAL_HOOK__.checkDCE)try{__REACT_DEVTOOLS_GLOBAL_HOOK__.checkDCE(e)}catch(t){console.error(t)}}(),e.exports=n(19)},,,,,function(e,t,n){\"use strict\";function r(e,t){return function(e){if(Array.isArray(e))return e}(e)||function(e,t){var n=[],r=!0,i=!1,l=void 0;try{for(var o,a=e[Symbol.iterator]();!(r=(o=a.next()).done)&&(n.push(o.value),!t||n.length!==t);r=!0);}catch(u){i=!0,l=u}finally{try{r||null==a.return||a.return()}finally{if(i)throw l}}return n}(e,t)||function(){throw new TypeError(\"Invalid attempt to destructure non-iterable instance\")}()}n.d(t,\"a\",function(){return r})},,function(e,t,n){\"use strict\";var r=n(10),i=\"function\"===typeof Symbol&&Symbol.for,l=i?Symbol.for(\"react.element\"):60103,o=i?Symbol.for(\"react.portal\"):60106,a=i?Symbol.for(\"react.fragment\"):60107,u=i?Symbol.for(\"react.strict_mode\"):60108,c=i?Symbol.for(\"react.profiler\"):60114,s=i?Symbol.for(\"react.provider\"):60109,f=i?Symbol.for(\"react.co[...string is too long...]");
			};
		}

		
		private void WebView_MessageEmitted(object sender, EventArgs<string> e)
		{
			string value = e.Value;
			string type = JsonUtility.FromJson<BridgeMessage>(value).type;
			if (!(type == "keyboard.inputReceived"))
			{
				if (!(type == "keyboard.initialized"))
				{
					return;
				}
				this._sendKeyboardLanguageMessage();
				if (this.Initialized != null)
				{
					this.Initialized(this, EventArgs.Empty);
				}
			}
			else
			{
				string val = StringBridgeMessage.ParseValue(value);
				if (this.InputReceived != null)
				{
					this.InputReceived(this, new EventArgs<string>(val));
				}
			}
		}

		
		private string _getKeyboardLanguage()
		{
			SystemLanguage systemLanguage = Application.systemLanguage;
			if (systemLanguage <= SystemLanguage.German)
			{
				if (systemLanguage == SystemLanguage.Danish)
				{
					return "da";
				}
				if (systemLanguage == SystemLanguage.French)
				{
					return "fr";
				}
				if (systemLanguage == SystemLanguage.German)
				{
					return "de";
				}
			}
			else if (systemLanguage <= SystemLanguage.Russian)
			{
				if (systemLanguage == SystemLanguage.Norwegian)
				{
					return "no";
				}
				if (systemLanguage == SystemLanguage.Russian)
				{
					return "ru";
				}
			}
			else
			{
				if (systemLanguage == SystemLanguage.Spanish)
				{
					return "es";
				}
				if (systemLanguage == SystemLanguage.Swedish)
				{
					return "sv";
				}
			}
			return "en";
		}

		
		private void _sendKeyboardLanguageMessage()
		{
			string data = JsonUtility.ToJson(new StringBridgeMessage
			{
				type = "keyboard.setLanguage",
				value = this._getKeyboardLanguage()
			});
			this.WebViewPrefab.WebView.PostMessage(data);
		}

		
		private const float DEFAULT_KEYBOARD_WIDTH = 0.5f;

		
		private const float DEFAULT_KEYBOARD_HEIGHT = 0.125f;
	}
}
