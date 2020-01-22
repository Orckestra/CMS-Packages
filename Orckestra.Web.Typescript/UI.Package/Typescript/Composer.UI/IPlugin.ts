module Orckestra.Composer {
    export interface IPlugin {
        initialize(window: Window, document: HTMLDocument): void;
    }
}
