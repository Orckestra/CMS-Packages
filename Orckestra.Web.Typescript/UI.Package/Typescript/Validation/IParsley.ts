module Orckestra.Composer {
    export interface IParsley {
        isValid(group?: any, force?: boolean): boolean;
        validate(group?: any, force?: boolean): boolean;
        destroy(): void;
    }
}
