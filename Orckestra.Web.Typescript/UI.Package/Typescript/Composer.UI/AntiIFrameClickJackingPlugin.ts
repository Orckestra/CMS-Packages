/// <reference path='./IPlugin.ts' />

module Orckestra.Composer {
    export class AntiIFrameClickJackingPlugin implements IPlugin {

        public initialize(window: Window, document: HTMLDocument): void {
            if (this.getOrigin(window.self) !== this.getOrigin(window.top)) {
                console.warn('This site cannot be hosted in an iFrame. Redirecting.');
                window.top.location.href = window.self.location.href;
            }
        }

        private getOrigin(window: Window) : string {
            var origin: string;

            if (!window.location['origin']) {
                window.location['origin'] = window.location.protocol + '//' + window.location.host;
            }

            origin = window.location['origin'];
            return origin;
        }
    }
}
