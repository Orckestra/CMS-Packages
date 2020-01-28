///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Composer.Analytics/Analytics/AnalyticsPlugin.ts' />

module Orckestra.Composer {
    (() => {

        'use strict';

        describe('WHEN a date is given in any format', () => {
            it('SHOULD convert to yyyy-mm-dd', () => {
                // assemble
                var plugin = new Orckestra.Composer.AnalyticsPlugin();
                var date = 'Fri Sep 22,2016';

                // act -- send to formatDate
                var formattedDate = plugin.formatDate(date);

                // assert -- does it match
                expect(formattedDate).toEqual('2016-09-22');
            });
        });
    })();
}