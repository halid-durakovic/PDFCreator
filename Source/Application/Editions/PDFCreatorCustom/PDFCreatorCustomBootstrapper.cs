﻿using System;
using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.GpoAdapter;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Editions.PDFCreatorCustom.Properties;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.Views;
using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.PDFCreatorCustom
{
    public class PDFCreatorCustomBootstrapper : Bootstrapper
    {
        protected override string EditionName => "PDFCreator Custom";
        protected override bool HideLicensing => true;
        protected override bool ShowWelcomeWindow => false;
        protected override ButtonDisplayOptions ButtonDisplayOptions => new ButtonDisplayOptions(true, true);

        public bool ValidOnTerminalServer => Customization.ApplyCustomization.Equals("true", StringComparison.InvariantCultureIgnoreCase);

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateAssistant, DisabledUpdateAssistant>();
        }

        protected override void RegisterActivationHelper(Container container)
        {
            container.RegisterSingleton<ILicenseServerHelper, UnlicensedLicenseServerHelper>();
            container.RegisterSingleton<IActivationHelper, UnlicensedActivationHelper>();
        }

        protected override void RegisterUserTokenExtractor(Container container)
        {
            container.Register<IPsParserFactory, PsParserFactory>();
            container.Register<IUserTokenExtractor, UserTokenExtractor>();
        }

        protected override IList<Type> GetStartupConditions(IList<Type> defaultConditions)
        {
            if (!ValidOnTerminalServer)
                defaultConditions.Add(typeof(TerminalServerNotAllowedCondition));

            return defaultConditions;
        }

        protected override ViewCustomization BuildCustomization()
        {
            if (Customization.ApplyCustomization.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                return new ViewCustomization(Customization.AboutDialog, Customization.MainForm, Customization.PrintJobWindowCaption, Customization.customlogo);
            }

            // TODO maybe throw exeption instead?
            return ViewCustomization.DefaultCustomization;
        }

        protected override SettingsProvider CreateSettingsProvider()
        {
            return new GpoAwareSettingsProvider();
        }
    }
}
