///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Composer.MyAccount/Common/MembershipService.ts' />

module Orckestra.Composer {
    'use strict';

    (() => {
        describe('WHEN MembershipService.login', () => {
            var membershipService: IMembershipService,
                composerClientStub: SinonStub;

            beforeEach(function() {
                composerClientStub = sinon.stub(Orckestra.Composer.ComposerClient, 'post', () => {
                    return Q.fcall(() => {
                        return true;
                    });
                });
                composerClientStub.withArgs('/api/membership/login');

                membershipService = new MembershipService(new MembershipRepository());
                membershipService.login({}, '');
            });

            afterEach(() => {
                composerClientStub.restore();
            });

            it('SHOULD call the login web api', () => {
                expect(composerClientStub.calledOnce).toBe(true);
            });
        });
    })();
}
