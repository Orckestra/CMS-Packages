module Orckestra.Composer {
    /**
     * Provides a mechanism for releasing any resources.
     */
    export interface IDisposable {
        dispose(): void;
    }
}
