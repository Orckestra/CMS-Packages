///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Composer.Cart/MonerisPaymentProvider/MonerisPaymentService.ts' />
///<reference path='../../../Typescript/Composer.Cart/MonerisPaymentProvider/ICreateVaultTokenOptions.ts' />

(() => {
    'use strict';

    describe('WHEN MonerisPaymentService.addCreditCard is invoked', () => {
        var service: Orckestra.Composer.MonerisPaymentService;
        var composerClientStub: SinonStub;
        var returnedValue: any;

        beforeEach(function(done) {
            service = new Orckestra.Composer.MonerisPaymentService();

            composerClientStub = sinon.stub(Orckestra.Composer.ComposerClient, 'post', function(url: string, data: any) {
                return Q({ });
            });

            composerClientStub.withArgs('/api/vaultprofile/addprofile', jasmine.any(Object));

            service.addCreditCard(<Orckestra.Composer.ICreateVaultTokenOptions> {})
                .done((val: any) => {
                    returnedValue = val;
                    composerClientStub.restore();
                    done();
                }, (reason: any) => {
                    fail(reason);
                });
        });

        it('SHOULD call ComposerClientStub', function() {
            expect(composerClientStub.called).toBe(true);
            expect(returnedValue).toBeTruthy();
        });
    });
})();
