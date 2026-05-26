using AMRent.Data.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AMRent.Data.Contexts
{
    public class FullDatabaseContext : DbContext
    {
        public FullDatabaseContext(DbContextOptions<FullDatabaseContext> options) : base(options)
        {

        }

        private void SaveChangesExtraLogic()
        {
            foreach (var entry in ChangeTracker.Entries<Models.Database.Reservation>())
            {
                if (entry.State == EntityState.Added)
                {
                    var entity = entry.Entity;
                    string counter = (Set<Models.Database.Reservation>().Count(x => x.CreateDate.Date == DateTime.Today.Date) + 1).ToString().PadLeft(3, '0');
                    string invertedYear = dCore.Helpers.String.Invert((DateTime.Today.Year % 100).ToString());
                    string invertedMonth = dCore.Helpers.String.Invert((DateTime.Today.Month).ToString("00"));
                    string invertedDay = dCore.Helpers.String.Invert((DateTime.Today.Day).ToString("00"));
                    entity.CreateDate = DateTime.Now;
                    entity.Number = $"R{entity.Source}{entity.CarSegment.Code}{counter}{invertedYear}{invertedMonth}{invertedDay}";
                }
            }

            foreach (var entry in ChangeTracker.Entries<Models.Database.Quotation>())
            {
                if (entry.State == EntityState.Added)
                {
                    var entity = entry.Entity;
                    string counter = (Set<Models.Database.Quotation>().Count(x => x.CreateDate.Date == DateTime.Today.Date) + 1).ToString().PadLeft(3, '0');
                    string invertedYear = dCore.Helpers.String.Invert((DateTime.Today.Year % 100).ToString());
                    string invertedMonth = dCore.Helpers.String.Invert((DateTime.Today.Month).ToString("00"));
                    string invertedDay = dCore.Helpers.String.Invert((DateTime.Today.Day).ToString("00"));
                    entity.CreateDate = DateTime.Now;
                    entity.Number = $"C{entity.Source}{counter}{invertedYear}{invertedMonth}{invertedDay}";
                }
            }
        }

        public void UpdateWithTracking(object entity, Guid userId)
        {
            Update(entity);

            var currentTime = DateTime.UtcNow;

            if (entity is Models.Database.Reservation
                || entity is Models.Database.Quotation
                || entity is Models.Database.Price
                || entity is Models.Database.Season
                || entity is Models.Database.PickupReturnLocationTax
                || entity is Models.Database.PickupReturnTemporaryTax)
            {
                object originalEntity = null;
                if (entity is Models.Database.Reservation reservation)
                {
                    originalEntity = Reservations.AsNoTracking().FirstOrDefault(x => x.Id == reservation.Id);
                }
                else if (entity is Models.Database.Quotation quotation)
                {
                    originalEntity = Quotations.AsNoTracking().FirstOrDefault(x => x.Id == quotation.Id);
                }
                else if (entity is Models.Database.Price price)
                {
                    originalEntity = Prices.AsNoTracking().FirstOrDefault(x => x.Id == price.Id);
                }
                else if (entity is Models.Database.Season season)
                {
                    originalEntity = Seasons.AsNoTracking().FirstOrDefault(x => x.Id == season.Id);
                }
                else if (entity is Models.Database.PickupReturnLocationTax pickupReturnLocationTax)
                {
                    originalEntity = PickupReturnLocationTaxes.AsNoTracking().FirstOrDefault(x => x.Id == pickupReturnLocationTax.Id);
                }
                else if (entity is Models.Database.PickupReturnTemporaryTax pickupReturnTemporaryTax)
                {
                    originalEntity = PickupReturnTemporaryTaxes.AsNoTracking().FirstOrDefault(x => x.Id == pickupReturnTemporaryTax.Id);
                }

                Entry(entity).OriginalValues.SetValues(originalEntity);

                foreach (var property in Entry(entity).Properties)
                {
                    Type propertyType = property.Metadata.ClrType;
                    string originalValue = $"{property.OriginalValue}";
                    string currentValue = $"{property.CurrentValue}";
                    if ((propertyType == typeof(decimal) && (decimal)property.OriginalValue != (decimal)property.CurrentValue)
                        || (propertyType != typeof(decimal) && originalValue != currentValue))
                    {
                        if (entity is Models.Database.Reservation)
                        {
                            ReservationChanges.Add(new Models.Database.ReservationChange
                            {
                                ReservationId = ((Models.Database.Reservation)entity).Id,
                                FieldName = property.Metadata.Name,
                                OldValue = originalValue,
                                NewValue = currentValue,
                                ChangeDateTime = currentTime,
                                UserId = userId
                            });
                        }
                        else if (entity is Models.Database.Quotation)
                        {
                            QuotationChanges.Add(new Models.Database.QuotationChange
                            {
                                QuotationId = ((Models.Database.Quotation)entity).Id,
                                FieldName = property.Metadata.Name,
                                OldValue = originalValue,
                                NewValue = currentValue,
                                ChangeDateTime = currentTime,
                                UserId = userId
                            });
                        }
                        else if (entity is Models.Database.Price)
                        {
                            PriceChanges.Add(new Models.Database.PriceChange
                            {
                                PriceId = ((Models.Database.Price)entity).Id,
                                FieldName = property.Metadata.Name,
                                OldValue = originalValue,
                                NewValue = currentValue,
                                ChangeDateTime = currentTime,
                                UserId = userId
                            });
                        }
                        else if (entity is Models.Database.Season)
                        {
                            SeasonChanges.Add(new Models.Database.SeasonChange
                            {
                                SeasonId = ((Models.Database.Season)entity).Id,
                                FieldName = property.Metadata.Name,
                                OldValue = originalValue,
                                NewValue = currentValue,
                                ChangeDateTime = currentTime,
                                UserId = userId
                            });
                        }
                        else if (entity is Models.Database.PickupReturnLocationTax)
                        {
                            PickupReturnLocationTaxChanges.Add(new Models.Database.PickupReturnLocationTaxChange
                            {
                                PickupReturnLocationTaxId = ((Models.Database.PickupReturnLocationTax)entity).Id,
                                FieldName = property.Metadata.Name,
                                OldValue = originalValue,
                                NewValue = currentValue,
                                ChangeDateTime = currentTime,
                                UserId = userId
                            });
                        }
                        else if (entity is Models.Database.PickupReturnTemporaryTax)
                        {
                            PickupReturnTemporaryTaxChanges.Add(new Models.Database.PickupReturnTemporaryTaxChange
                            {
                                PickupReturnTemporaryTaxId = ((Models.Database.PickupReturnTemporaryTax)entity).Id,
                                FieldName = property.Metadata.Name,
                                OldValue = originalValue,
                                NewValue = currentValue,
                                ChangeDateTime = currentTime,
                                UserId = userId
                            });
                        }
                    }
                }
                if (entity is Models.Database.Reservation)
                {
                    foreach (Models.Database.DataProtectionConsentReservation childEntity in ((Models.Database.Reservation)entity).DataProtectionConsents)
                    {
                        object originalChildEntity = DataProtectionConsentReservations.AsNoTracking().FirstOrDefault(x => x.Id == childEntity.Id);
                        if (originalChildEntity == null)
                        {
                            continue;
                        }
                        Entry(childEntity).OriginalValues.SetValues(originalChildEntity);

                        foreach (var childProperty in Entry(childEntity).Properties)
                        {
                            Type childPropertyType = childProperty.Metadata.ClrType;
                            string originalChildValue = $"{childProperty.OriginalValue}";
                            string currentChildValue = $"{childProperty.CurrentValue}";
                            if ((childPropertyType == typeof(decimal) && (decimal)childProperty.OriginalValue != (decimal)childProperty.CurrentValue) || (childPropertyType != typeof(decimal) && originalChildValue != currentChildValue))
                            {
                                childEntity.DataProtectionConsentReservationChanges.Add(new Models.Database.DataProtectionConsentReservationChange
                                {
                                    FieldName = childProperty.Metadata.Name,
                                    OldValue = originalChildValue,
                                    NewValue = currentChildValue,
                                    ChangeDateTime = currentTime,
                                    UserId = userId
                                });
                            }
                        }
                    }
                }
                if (entity is Models.Database.Quotation)
                {
                    foreach (Models.Database.DataProtectionConsentQuotation childEntity in ((Models.Database.Quotation)entity).DataProtectionConsents)
                    {
                        object originalChildEntity = DataProtectionConsentQuotations.AsNoTracking().FirstOrDefault(x => x.Id == childEntity.Id);
                        if (originalChildEntity == null)
                        {
                            continue;
                        }
                        Entry(childEntity).OriginalValues.SetValues(originalChildEntity);

                        foreach (var childProperty in Entry(childEntity).Properties)
                        {
                            Type childPropertyType = childProperty.Metadata.ClrType;
                            string originalChildValue = $"{childProperty.OriginalValue}";
                            string currentChildValue = $"{childProperty.CurrentValue}";
                            if ((childPropertyType == typeof(decimal) && (decimal)childProperty.OriginalValue != (decimal)childProperty.CurrentValue) || (childPropertyType != typeof(decimal) && originalChildValue != currentChildValue))
                            {
                                childEntity.DataProtectionConsentQuotationChanges.Add(new Models.Database.DataProtectionConsentQuotationChange
                                {
                                    FieldName = childProperty.Metadata.Name,
                                    OldValue = originalChildValue,
                                    NewValue = currentChildValue,
                                    ChangeDateTime = currentTime,
                                    UserId = userId
                                });
                            }
                        }
                    }
                }
            }
            else if (entity is Models.Database.CarSegment)
            {
                foreach (Models.Database.Insurance childEntity in ((Models.Database.CarSegment)entity).Insurances)
                {
                    object originalEntity = Insurances.AsNoTracking().FirstOrDefault(x => x.CarSegmentId == childEntity.CarSegmentId && x.InsuranceLevelId == childEntity.InsuranceLevelId);
                    if (originalEntity == null)
                    {
                        continue;
                    }
                    Entry(childEntity).OriginalValues.SetValues(originalEntity);

                    foreach (var property in Entry(childEntity).Properties)
                    {
                        Type propertyType = property.Metadata.ClrType;
                        string originalValue = $"{property.OriginalValue}";
                        string currentValue = $"{property.CurrentValue}";
                        if ((propertyType == typeof(decimal) && (decimal)property.OriginalValue != (decimal)property.CurrentValue) || (propertyType != typeof(decimal) && originalValue != currentValue))
                        {
                            childEntity.Changes.Add(new Models.Database.InsuranceChange
                            {
                                FieldName = property.Metadata.Name,
                                OldValue = originalValue,
                                NewValue = currentValue,
                                ChangeDateTime = currentTime,
                                UserId = userId
                            });
                        }
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            SaveChangesExtraLogic();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesExtraLogic();
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // ##### Relations #####

            // Translations
            modelBuilder.Entity<Models.Database.About>().HasMany(x => x.Translations).WithOne(x => x.About).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.BlogArticle>().HasMany(x => x.Translations).WithOne(x => x.BlogArticle).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.BlogArticleCategory>().HasMany(x => x.Translations).WithOne(x => x.BlogArticleCategory).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.Campaign>().HasMany(x => x.Translations).WithOne(x => x.Campaign).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.CarCategory>().HasMany(x => x.Translations).WithOne(x => x.CarCategory).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.CarFuel>().HasMany(x => x.Translations).WithOne(x => x.CarFuel).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.CarGearbox>().HasMany(x => x.Translations).WithOne(x => x.CarGearbox).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.CarSegment>().HasMany(x => x.Translations).WithOne(x => x.CarSegment).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.Country>().HasMany(x => x.Translations).WithOne(x => x.Country).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.DataProtectionConsent>().HasMany(x => x.Translations).WithOne(x => x.DataProtectionConsent).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.EmailContent>().HasMany(x => x.Translations).WithOne(x => x.EmailContent).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.Extra>().HasMany(x => x.Translations).WithOne(x => x.Extra).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.Fact>().HasMany(x => x.Translations).WithOne(x => x.Fact).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<dCore.MultiLanguage.Models.GenericText>().HasMany(x => x.Translations).WithOne(x => x.GenericText).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.HomeBanner>().HasMany(x => x.Translations).WithOne(x => x.HomeBanner).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.InsuranceLevel>().HasMany(x => x.Translations).WithOne(x => x.InsuranceLevel).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.OfficeLocation>().HasMany(x => x.Translations).WithOne(x => x.OfficeLocation).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.PickupReturnLocation>().HasMany(x => x.Translations).WithOne(x => x.PickupReturnLocation).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.Process>().HasMany(x => x.Translations).WithOne(x => x.Process).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.QuestionAndAnswer>().HasMany(x => x.Translations).WithOne(x => x.QuestionAndAnswer).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.ReasonToChooseUs>().HasMany(x => x.Translations).WithOne(x => x.ReasonToChooseUs).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.Service>().HasMany(x => x.Translations).WithOne(x => x.Service).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.TeamMember>().HasMany(x => x.Translations).WithOne(x => x.TeamMember).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.Testimonial>().HasMany(x => x.Translations).WithOne(x => x.Testimonial).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.TranslatableSetting>().HasMany(x => x.Translations).WithOne(x => x.TranslatableSetting).OnDelete(DeleteBehavior.Cascade);

            // Specific One to Many
            modelBuilder.Entity<Models.Database.Insurance>().HasMany(x => x.Changes).WithOne(x => x.Insurance).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Models.Database.PickupReturnLocation>().HasMany(x => x.PickupReservations).WithOne(x => x.PickupLocation).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Models.Database.PickupReturnLocation>().HasMany(x => x.ReturnReservations).WithOne(x => x.ReturnLocation).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Models.Database.PickupReturnLocation>().HasMany(x => x.PickupQuotations).WithOne(x => x.PickupLocation).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Models.Database.PickupReturnLocation>().HasMany(x => x.ReturnQuotations).WithOne(x => x.ReturnLocation).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Models.Database.PickupReturnLocation>().HasMany(x => x.DayOfWeekSchedules).WithOne(x => x.PickupReturnLocation).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Database.User>().HasMany(x => x.AssignedReservations).WithOne(x => x.AssignedUser).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Models.Database.User>().HasMany(x => x.Reservations).WithOne(x => x.Customer).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Models.Database.User>().HasMany(x => x.Quotations).WithOne(x => x.User).OnDelete(DeleteBehavior.NoAction);

            // Many to Many
            modelBuilder.Entity<Models.Database.Campaign>()
                .HasMany(x => x.CarSegments)
                .WithMany(x => x.Campaigns)
                .UsingEntity<Models.Database.CampaignCarSegment>();
            modelBuilder.Entity<Models.Database.Campaign>()
                .HasMany(x => x.Extras)
                .WithMany(x => x.Campaigns)
                .UsingEntity<Models.Database.CampaignExtra>();
            modelBuilder.Entity<Models.Database.Voucher>()
                .HasMany(x => x.Extras)
                .WithMany(x => x.Vouchers)
                .UsingEntity<Models.Database.VoucherExtra>();

            // ##### Composite Keys #####

            modelBuilder.Entity<Models.Database.AboutTranslation>().HasKey(x => new { x.LanguageId, x.AboutId });
            modelBuilder.Entity<Models.Database.BlogArticleTranslation>().HasKey(x => new { x.LanguageId, x.BlogArticleId });
            modelBuilder.Entity<Models.Database.BlogArticleCategoryTranslation>().HasKey(x => new { x.LanguageId, x.BlogArticleCategoryId });
            modelBuilder.Entity<Models.Database.CampaignTranslation>().HasKey(x => new { x.LanguageId, x.CampaignId });
            modelBuilder.Entity<Models.Database.CampaignCarSegment>().HasKey(x => new { x.CarSegmentId, x.CampaignId });
            modelBuilder.Entity<Models.Database.CampaignExtra>().HasKey(x => new { x.ExtraId, x.CampaignId });
            modelBuilder.Entity<Models.Database.CarCategoryTranslation>().HasKey(x => new { x.LanguageId, x.CarCategoryId });
            modelBuilder.Entity<Models.Database.CarFuelTranslation>().HasKey(x => new { x.LanguageId, x.CarFuelId });
            modelBuilder.Entity<Models.Database.CarGearboxTranslation>().HasKey(x => new { x.LanguageId, x.CarGearboxId });
            modelBuilder.Entity<Models.Database.CarSegmentTranslation>().HasKey(x => new { x.LanguageId, x.CarSegmentId });
            modelBuilder.Entity<Models.Database.CountryTranslation>().HasKey(x => new { x.LanguageId, x.CountryId });
            modelBuilder.Entity<Models.Database.DataProtectionConsentTranslation>().HasKey(x => new { x.LanguageId, x.DataProtectionConsentId });
            modelBuilder.Entity<Models.Database.EmailContentTranslation>().HasKey(x => new { x.LanguageId, x.EmailContentId });
            modelBuilder.Entity<Models.Database.ExtraPriceByInsuranceLevel>().HasKey(x => new { x.InsuranceLevelId, x.ExtraId });
            modelBuilder.Entity<Models.Database.ExtraTranslation>().HasKey(x => new { x.LanguageId, x.ExtraId });
            modelBuilder.Entity<Models.Database.FactTranslation>().HasKey(x => new { x.LanguageId, x.FactId });
            modelBuilder.Entity<dCore.MultiLanguage.Models.GenericTextTranslation>().HasKey(x => new { x.LanguageId, x.GenericTextId });
            modelBuilder.Entity<Models.Database.HomeBannerTranslation>().HasKey(x => new { x.LanguageId, x.HomeBannerId });
            modelBuilder.Entity<Models.Database.InsuranceLevelTranslation>().HasKey(x => new { x.LanguageId, x.InsuranceLevelId });
            modelBuilder.Entity<Models.Database.OfficeLocationTranslation>().HasKey(x => new { x.LanguageId, x.OfficeLocationId });
            modelBuilder.Entity<Models.Database.PickupReturnLocationDayOfWeekSchedule>().HasKey(x => new { x.DayOfWeek, x.PickupReturnLocationId });
            modelBuilder.Entity<Models.Database.PickupReturnLocationTranslation>().HasKey(x => new { x.LanguageId, x.PickupReturnLocationId });
            modelBuilder.Entity<Models.Database.PickupReturnTemporaryTaxTranslation>().HasKey(x => new { x.LanguageId, x.PickupReturnTemporaryTaxId });
            modelBuilder.Entity<Models.Database.ProcessTranslation>().HasKey(x => new { x.LanguageId, x.ProcessId });
            modelBuilder.Entity<Models.Database.QuestionAndAnswerTranslation>().HasKey(x => new { x.LanguageId, x.QuestionAndAnswerId });
            modelBuilder.Entity<Models.Database.QuotationItemExtra>().HasKey(x => new { x.ExtraId, x.QuotationItemId });
            modelBuilder.Entity<Models.Database.QuotationItemPickupReturnTemporaryTax>().HasKey(x => new { x.QuotationItemId, x.PickupReturnTemporaryTaxId });
            modelBuilder.Entity<Models.Database.QuotationItemService>().HasKey(x => new { x.ServiceId, x.QuotationItemId });
            modelBuilder.Entity<Models.Database.ReasonToChooseUsTranslation>().HasKey(x => new { x.LanguageId, x.ReasonToChooseUsId });
            modelBuilder.Entity<Models.Database.ReservationExtra>().HasKey(x => new { x.ExtraId, x.ReservationId });
            modelBuilder.Entity<Models.Database.ReservationPickupReturnTemporaryTax>().HasKey(x => new { x.ReservationId, x.PickupReturnTemporaryTaxId });
            modelBuilder.Entity<Models.Database.ReservationService>().HasKey(x => new { x.ServiceId, x.ReservationId });
            modelBuilder.Entity<Models.Database.ServiceTranslation>().HasKey(x => new { x.LanguageId, x.ServiceId });
            modelBuilder.Entity<Models.Database.TeamMemberTranslation>().HasKey(x => new { x.LanguageId, x.TeamMemberId });
            modelBuilder.Entity<Models.Database.TestimonialTranslation>().HasKey(x => new { x.LanguageId, x.TestimonialId });
            modelBuilder.Entity<Models.Database.TranslatableSettingTranslation>().HasKey(x => new { x.LanguageId, x.TranslatableSettingId });
            modelBuilder.Entity<Models.Database.VoucherExtra>().HasKey(x => new { x.ExtraId, x.VoucherId });

            // ##### Constraints #####

            modelBuilder.Entity<dCore.MultiLanguage.Models.GenericText>().HasIndex(x => new { x.Key }).IsUnique();
            modelBuilder.Entity<Models.Database.Insurance>().HasIndex(x => new { x.InsuranceLevelId, x.CarSegmentId }).IsUnique();
            modelBuilder.Entity<dCore.MultiLanguage.Models.Language>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<dCore.MultiLanguage.Models.Language>().HasIndex(x => new { x.Code, x.CountryCode }).IsUnique();
            modelBuilder.Entity<Models.Database.PickupReturnLocationTax>().HasIndex(x => new { x.PickupReturnLocationId, x.Days }).IsUnique();
            modelBuilder.Entity<Models.Database.Price>().HasIndex(x => new { x.SeasonId, x.CarSegmentId, x.Days }).IsUnique();
            modelBuilder.Entity<Models.Database.SocialNetwork>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Models.Database.Voucher>().HasIndex(x => x.Code).IsUnique();

            // ##### Table Names #####

            // Identity
            modelBuilder.Entity<Models.Database.User>()
                .HasMany(x => x.Roles)
                .WithMany(x => x.Users)
                .UsingEntity<Models.Database.RoleUser>();
            modelBuilder.Entity<Models.Database.User>().ToTable(nameof(Models.Database.User));
            modelBuilder.Entity<Models.Database.User>().HasIndex(x => x.Username).IsUnique();
            modelBuilder.Entity<Models.Database.Role>().ToTable(nameof(Models.Database.Role));
            modelBuilder.Entity<Models.Database.RoleUser>().ToTable(nameof(Models.Database.RoleUser));
            modelBuilder.Entity<Models.Database.RoleUser>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<Models.Database.RolePermission>().ToTable(nameof(Models.Database.RolePermission));
            modelBuilder.Entity<Models.Database.RolePermission>().HasKey(x => new { x.RoleId, x.Permission });

            // Business
            modelBuilder.Entity<Models.Database.About>().ToTable(nameof(Models.Database.About));
            modelBuilder.Entity<Models.Database.BlogArticle>().ToTable(nameof(Models.Database.BlogArticle));
            modelBuilder.Entity<Models.Database.BlogArticleTranslation>().ToTable(nameof(Models.Database.BlogArticleTranslation));
            modelBuilder.Entity<Models.Database.BlogArticleCategory>().ToTable(nameof(Models.Database.BlogArticleCategory));
            modelBuilder.Entity<Models.Database.BlogArticleCategoryTranslation>().ToTable(nameof(Models.Database.BlogArticleCategoryTranslation));
            modelBuilder.Entity<Models.Database.Campaign>().ToTable(nameof(Models.Database.Campaign));
            modelBuilder.Entity<Models.Database.CampaignCarSegment>().ToTable(nameof(Models.Database.CampaignCarSegment));
            modelBuilder.Entity<Models.Database.CampaignExtra>().ToTable(nameof(Models.Database.CampaignExtra));
            modelBuilder.Entity<Models.Database.CampaignTranslation>().ToTable(nameof(Models.Database.CampaignTranslation));
            modelBuilder.Entity<Models.Database.CarCategory>().ToTable(nameof(Models.Database.CarCategory));
            modelBuilder.Entity<Models.Database.CarCategoryTranslation>().ToTable(nameof(Models.Database.CarCategoryTranslation));
            modelBuilder.Entity<Models.Database.CarFuel>().ToTable(nameof(Models.Database.CarFuel));
            modelBuilder.Entity<Models.Database.CarFuelTranslation>().ToTable(nameof(Models.Database.CarFuelTranslation));
            modelBuilder.Entity<Models.Database.CarGearbox>().ToTable(nameof(Models.Database.CarGearbox));
            modelBuilder.Entity<Models.Database.CarGearboxTranslation>().ToTable(nameof(Models.Database.CarGearboxTranslation));
            modelBuilder.Entity<Models.Database.CarSegment>().ToTable(nameof(Models.Database.CarSegment));
            modelBuilder.Entity<Models.Database.CarSegmentTranslation>().ToTable(nameof(Models.Database.CarSegmentTranslation));
            modelBuilder.Entity<Models.Database.Country>().ToTable(nameof(Models.Database.Country));
            modelBuilder.Entity<Models.Database.CountryTranslation>().ToTable(nameof(Models.Database.CountryTranslation));
            modelBuilder.Entity<Models.Database.DataProtectionConsent>().ToTable(nameof(Models.Database.DataProtectionConsent));
            modelBuilder.Entity<Models.Database.DataProtectionConsentQuotation>().ToTable(nameof(Models.Database.DataProtectionConsentQuotation));
            modelBuilder.Entity<Models.Database.DataProtectionConsentQuotationChange>().ToTable(nameof(Models.Database.DataProtectionConsentQuotationChange));
            modelBuilder.Entity<Models.Database.DataProtectionConsentReservation>().ToTable(nameof(Models.Database.DataProtectionConsentReservation));
            modelBuilder.Entity<Models.Database.DataProtectionConsentReservationChange>().ToTable(nameof(Models.Database.DataProtectionConsentReservationChange));
            modelBuilder.Entity<Models.Database.DataProtectionConsentTranslation>().ToTable(nameof(Models.Database.DataProtectionConsentTranslation));
            modelBuilder.Entity<Models.Database.DataProtectionConsentUser>().ToTable(nameof(Models.Database.DataProtectionConsentUser));
            modelBuilder.Entity<Models.Database.EmailContent>().ToTable(nameof(Models.Database.EmailContent));
            modelBuilder.Entity<Models.Database.EmailContentTranslation>().ToTable(nameof(Models.Database.EmailContentTranslation));
            modelBuilder.Entity<Models.Database.Extra>().ToTable(nameof(Models.Database.Extra));
            modelBuilder.Entity<Models.Database.ExtraPriceByInsuranceLevel>().ToTable(nameof(Models.Database.ExtraPriceByInsuranceLevel));
            modelBuilder.Entity<Models.Database.ExtraTranslation>().ToTable(nameof(Models.Database.ExtraTranslation));
            modelBuilder.Entity<Models.Database.Fact>().ToTable(nameof(Models.Database.Fact));
            modelBuilder.Entity<Models.Database.FactTranslation>().ToTable(nameof(Models.Database.FactTranslation));
            modelBuilder.Entity<dCore.MultiLanguage.Models.GenericText>().ToTable(nameof(dCore.MultiLanguage.Models.GenericText));
            modelBuilder.Entity<dCore.MultiLanguage.Models.GenericTextTranslation>().ToTable(nameof(dCore.MultiLanguage.Models.GenericTextTranslation));
            modelBuilder.Entity<Models.Database.HomeBanner>().ToTable(nameof(Models.Database.HomeBanner));
            modelBuilder.Entity<Models.Database.HomeBannerTranslation>().ToTable(nameof(Models.Database.HomeBannerTranslation));
            modelBuilder.Entity<Models.Database.Insurance>().ToTable(nameof(Models.Database.Insurance));
            modelBuilder.Entity<Models.Database.InsuranceChange>().ToTable(nameof(Models.Database.InsuranceChange));
            modelBuilder.Entity<Models.Database.InsuranceLevel>().ToTable(nameof(Models.Database.InsuranceLevel));
            modelBuilder.Entity<Models.Database.InsuranceLevelTranslation>().ToTable(nameof(Models.Database.InsuranceLevelTranslation));
            modelBuilder.Entity<Models.Database.InsurancePrice>().ToTable(nameof(Models.Database.InsurancePrice));
            modelBuilder.Entity<dCore.MultiLanguage.Models.Language>().ToTable(nameof(dCore.MultiLanguage.Models.Language));
            modelBuilder.Entity<Models.Database.OfficeLocation>().ToTable(nameof(Models.Database.OfficeLocation));
            modelBuilder.Entity<Models.Database.OfficeLocationTranslation>().ToTable(nameof(Models.Database.OfficeLocationTranslation));
            modelBuilder.Entity<Models.Database.PickupReturnLocation>().ToTable(nameof(Models.Database.PickupReturnLocation));
            modelBuilder.Entity<Models.Database.PickupReturnLocationDayOfWeekSchedule>().ToTable(nameof(Models.Database.PickupReturnLocationDayOfWeekSchedule));
            modelBuilder.Entity<Models.Database.PickupReturnLocationTax>().ToTable(nameof(Models.Database.PickupReturnLocationTax));
            modelBuilder.Entity<Models.Database.PickupReturnLocationTaxChange>().ToTable(nameof(Models.Database.PickupReturnLocationTaxChange));
            modelBuilder.Entity<Models.Database.PickupReturnLocationTranslation>().ToTable(nameof(Models.Database.PickupReturnLocationTranslation));
            modelBuilder.Entity<Models.Database.PickupReturnTemporaryTax>().ToTable(nameof(Models.Database.PickupReturnTemporaryTax));
            modelBuilder.Entity<Models.Database.PickupReturnTemporaryTaxChange>().ToTable(nameof(Models.Database.PickupReturnTemporaryTaxChange));
            modelBuilder.Entity<Models.Database.Price>().ToTable(nameof(Models.Database.Price));
            modelBuilder.Entity<Models.Database.PriceChange>().ToTable(nameof(Models.Database.PriceChange));
            modelBuilder.Entity<Models.Database.Process>().ToTable(nameof(Models.Database.Process));
            modelBuilder.Entity<Models.Database.ProcessTranslation>().ToTable(nameof(Models.Database.ProcessTranslation));
            modelBuilder.Entity<Models.Database.QuestionAndAnswer>().ToTable(nameof(Models.Database.QuestionAndAnswer));
            modelBuilder.Entity<Models.Database.QuestionAndAnswerTranslation>().ToTable(nameof(Models.Database.QuestionAndAnswerTranslation));
            modelBuilder.Entity<Models.Database.Quotation>().ToTable(nameof(Models.Database.Quotation));
            modelBuilder.Entity<Models.Database.QuotationChange>().ToTable(nameof(Models.Database.QuotationChange));
            modelBuilder.Entity<Models.Database.QuotationItem>().ToTable(nameof(Models.Database.QuotationItem));
            modelBuilder.Entity<Models.Database.QuotationItemExtra>().ToTable(nameof(Models.Database.QuotationItemExtra));
            modelBuilder.Entity<Models.Database.QuotationItemPickupReturnTemporaryTax>().ToTable(nameof(Models.Database.QuotationItemPickupReturnTemporaryTax));
            modelBuilder.Entity<Models.Database.QuotationItemService>().ToTable(nameof(Models.Database.QuotationItemService));
            modelBuilder.Entity<Models.Database.ReasonToChooseUs>().ToTable(nameof(Models.Database.ReasonToChooseUs));
            modelBuilder.Entity<Models.Database.ReasonToChooseUsTranslation>().ToTable(nameof(Models.Database.ReasonToChooseUsTranslation));
            modelBuilder.Entity<Models.Database.Reservation>().ToTable(nameof(Models.Database.Reservation));
            modelBuilder.Entity<Models.Database.ReservationChange>().ToTable(nameof(Models.Database.ReservationChange));
            modelBuilder.Entity<Models.Database.ReservationExtra>().ToTable(nameof(Models.Database.ReservationExtra));
            modelBuilder.Entity<Models.Database.ReservationExtraDriver>().ToTable(nameof(Models.Database.ReservationExtraDriver));
            modelBuilder.Entity<Models.Database.ReservationService>().ToTable(nameof(Models.Database.ReservationService));
            modelBuilder.Entity<Models.Database.ReservationPickupReturnTemporaryTax>().ToTable(nameof(Models.Database.ReservationPickupReturnTemporaryTax));
            modelBuilder.Entity<Models.Database.ReservationQuotationCancellationReason>().ToTable(nameof(Models.Database.ReservationQuotationCancellationReason));
            modelBuilder.Entity<Models.Database.Season>().ToTable(nameof(Models.Database.Season));
            modelBuilder.Entity<Models.Database.SeasonChange>().ToTable(nameof(Models.Database.SeasonChange));
            modelBuilder.Entity<Models.Database.SeasonCategory>().ToTable(nameof(Models.Database.SeasonCategory));
            modelBuilder.Entity<Models.Database.Service>().ToTable(nameof(Models.Database.Service));
            modelBuilder.Entity<Models.Database.ServiceTranslation>().ToTable(nameof(Models.Database.ServiceTranslation));
            modelBuilder.Entity<Models.Database.Setting>().ToTable(nameof(Models.Database.Setting));
            modelBuilder.Entity<Models.Database.Setting>().ToTable(nameof(Models.Database.Setting));
            modelBuilder.Entity<Models.Database.SocialNetwork>().ToTable(nameof(Models.Database.SocialNetwork));
            modelBuilder.Entity<Models.Database.TeamMember>().ToTable(nameof(Models.Database.TeamMember));
            modelBuilder.Entity<Models.Database.TeamMemberTranslation>().ToTable(nameof(Models.Database.TeamMemberTranslation));
            modelBuilder.Entity<Models.Database.Testimonial>().ToTable(nameof(Models.Database.Testimonial));
            modelBuilder.Entity<Models.Database.TestimonialTranslation>().ToTable(nameof(Models.Database.TestimonialTranslation));
            modelBuilder.Entity<Models.Database.TranslatableSetting>().ToTable(nameof(Models.Database.TranslatableSetting));
            modelBuilder.Entity<Models.Database.TranslatableSettingTranslation>().ToTable(nameof(Models.Database.TranslatableSettingTranslation));
            modelBuilder.Entity<Models.Database.Voucher>().ToTable(nameof(Models.Database.Voucher));
            modelBuilder.Entity<Models.Database.VoucherExtra>().ToTable(nameof(Models.Database.VoucherExtra));

            // ##### Default Values #####
            modelBuilder.Entity<Models.Database.EmailContent>().Property(e => e.SendQuotationReservationSummaryPdf).HasDefaultValue(false);
            modelBuilder.Entity<Models.Database.InsurancePrice>().Property(e => e.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Models.Database.User>().Property(e => e.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Models.Database.ViaVerdeMovement>().Property(e => e.Id).HasDefaultValueSql("NEWID()");

            // ##### Master Data Seed #####

            // Roles
            modelBuilder.Entity<Models.Database.Role>().HasData(new Models.Database.Role() { Id = (int)Enums.UserRoles.Customer, Description = "Cliente", Name = "Utilizador" });
            modelBuilder.Entity<Models.Database.Role>().HasData(new Models.Database.Role() { Id = (int)Enums.UserRoles.Developer, Description = "Developer", Name = "Developer" });
            modelBuilder.Entity<Models.Database.Role>().HasData(new Models.Database.Role() { Id = (int)Enums.UserRoles.Administrator, Description = "Administração", Name = "Administrador" });
            modelBuilder.Entity<Models.Database.Role>().HasData(new Models.Database.Role() { Id = (int)Enums.UserRoles.Marketing, Description = "Marketing", Name = "Marketing" });
            modelBuilder.Entity<Models.Database.Role>().HasData(new Models.Database.Role() { Id = (int)Enums.UserRoles.Renting, Description = "Renting", Name = "Renting" });
            modelBuilder.Entity<Models.Database.Role>().HasData(new Models.Database.Role() { Id = (int)Enums.UserRoles.Administrative, Description = "Reservas", Name = "Reservas" });

            // Role Permissions
            modelBuilder.Entity<Models.Database.RolePermission>().HasData(new Models.Database.RolePermission() { RoleId = 2, Permission = Enums.Permissions.Other });
            modelBuilder.Entity<Models.Database.RolePermission>().HasData(new Models.Database.RolePermission() { RoleId = 2, Permission = Enums.Permissions.Reservations });
            modelBuilder.Entity<Models.Database.RolePermission>().HasData(new Models.Database.RolePermission() { RoleId = 3, Permission = Enums.Permissions.Other });
            modelBuilder.Entity<Models.Database.RolePermission>().HasData(new Models.Database.RolePermission() { RoleId = 3, Permission = Enums.Permissions.Reservations });
            modelBuilder.Entity<Models.Database.RolePermission>().HasData(new Models.Database.RolePermission() { RoleId = 4, Permission = Enums.Permissions.Other });
            modelBuilder.Entity<Models.Database.RolePermission>().HasData(new Models.Database.RolePermission() { RoleId = 4, Permission = Enums.Permissions.Reservations });
            modelBuilder.Entity<Models.Database.RolePermission>().HasData(new Models.Database.RolePermission() { RoleId = 5, Permission = Enums.Permissions.Other });
            modelBuilder.Entity<Models.Database.RolePermission>().HasData(new Models.Database.RolePermission() { RoleId = 5, Permission = Enums.Permissions.Reservations });
            modelBuilder.Entity<Models.Database.RolePermission>().HasData(new Models.Database.RolePermission() { RoleId = 6, Permission = Enums.Permissions.Reservations });

            // Users
            modelBuilder.Entity<Models.Database.User>().HasData(new Models.Database.User() { Id = Guid.Parse("3f1d2e57-4a77-44d9-9c0d-5b2e3c2a1e99"), EmailAddress = "dalmeida@daga.pt", Username = "dalmeida", Password = "XrfLK0rKzCYfTR3yNwyLRQ==", FirstName = "Diogo", LastName = "Almeida" });
            modelBuilder.Entity<Models.Database.User>().HasData(new Models.Database.User() { Id = Guid.Parse("e6f8a9b1-0d3c-465e-90b8-2f9e2d4a8c11"), EmailAddress = "alexandra.santos@amrent.com", Username = "alexandra.santos", Password = dCore.Cryptography.Providers.Generic.Encrypt("SaRaFaUtO!2024"), FirstName = "Alexandra", LastName = "Santos" });
            modelBuilder.Entity<Models.Database.User>().HasData(new Models.Database.User() { Id = Guid.Parse("9bfa3d47-2c5e-4f38-9872-7b6d4e1c5a99"), EmailAddress = "sara.rodrigues@amrent.com", Username = "sara.rodrigues", Password = dCore.Cryptography.Providers.Generic.Encrypt("SaRaFaUtO!2024"), FirstName = "Sara", LastName = "Rodrigues" });
            modelBuilder.Entity<Models.Database.User>().HasData(new Models.Database.User() { Id = Guid.Parse("42a7b5d3-98e0-4e19-bd89-1c5f3e0d2a67"), EmailAddress = "elsa.francisco@amrent.pt", Username = "elsa.francisco", Password = dCore.Cryptography.Providers.Generic.Encrypt("SaRaFaUtO!2024"), FirstName = "Elsa", LastName = "Francisco" });
            modelBuilder.Entity<Models.Database.User>().HasData(new Models.Database.User() { Id = Guid.Parse("d51c7e89-3a2b-417c-8f0d-5e1b9c6d4f22"), EmailAddress = "joana.leitao@amrent.pt", Username = "joana.leitao", Password = dCore.Cryptography.Providers.Generic.Encrypt("SaRaFaUtO!2024"), FirstName = "Joana", LastName = "Leitão" });
            modelBuilder.Entity<Models.Database.User>().HasData(new Models.Database.User() { Id = Guid.Parse("5a9e2c3d-7b0f-4e81-bd46-3f2d1a5e98c7"), EmailAddress = "dina.gomes@amrent.pt", Username = "dina.gomes", Password = dCore.Cryptography.Providers.Generic.Encrypt("SaRaFaUtO!2024"), FirstName = "Dina", LastName = "Gomes" });
            modelBuilder.Entity<Models.Database.User>().HasData(new Models.Database.User() { Id = Guid.Parse("f8b6d3a1-9c42-4e70-83d2-5e9a1b7c4f55"), EmailAddress = "sabina.gameiro@amconfraria.com", Username = "sabina.gameiro", Password = dCore.Cryptography.Providers.Generic.Encrypt("SaRaFaUtO!2024"), FirstName = "Sabina", LastName = "Gameiro" });

            // User Roles
            modelBuilder.Entity<Models.Database.RoleUser>().HasData(new Models.Database.RoleUser() { RoleId = 2, UserId = Guid.Parse("3f1d2e57-4a77-44d9-9c0d-5b2e3c2a1e99") });
            modelBuilder.Entity<Models.Database.RoleUser>().HasData(new Models.Database.RoleUser() { RoleId = 3, UserId = Guid.Parse("e6f8a9b1-0d3c-465e-90b8-2f9e2d4a8c11") });
            modelBuilder.Entity<Models.Database.RoleUser>().HasData(new Models.Database.RoleUser() { RoleId = 4, UserId = Guid.Parse("9bfa3d47-2c5e-4f38-9872-7b6d4e1c5a99") });
            modelBuilder.Entity<Models.Database.RoleUser>().HasData(new Models.Database.RoleUser() { RoleId = 5, UserId = Guid.Parse("42a7b5d3-98e0-4e19-bd89-1c5f3e0d2a67") });
            modelBuilder.Entity<Models.Database.RoleUser>().HasData(new Models.Database.RoleUser() { RoleId = 6, UserId = Guid.Parse("d51c7e89-3a2b-417c-8f0d-5e1b9c6d4f22") });
            modelBuilder.Entity<Models.Database.RoleUser>().HasData(new Models.Database.RoleUser() { RoleId = 6, UserId = Guid.Parse("5a9e2c3d-7b0f-4e81-bd46-3f2d1a5e98c7") });
            modelBuilder.Entity<Models.Database.RoleUser>().HasData(new Models.Database.RoleUser() { RoleId = 6, UserId = Guid.Parse("f8b6d3a1-9c42-4e70-83d2-5e9a1b7c4f55") });

            // About
            modelBuilder.Entity<Models.Database.About>().HasData(new Models.Database.About() { Id = 1 });

            // Settings
            modelBuilder.Entity<Models.Database.Setting>().HasData(new Models.Database.Setting() { Id = 1, Key = "QuotationExpiringReminderDays", Description = "Número de dias de antecedência para notificar por email que uma cotação está prestes a expirar", Value = "5" });
            modelBuilder.Entity<Models.Database.Setting>().HasData(new Models.Database.Setting() { Id = 2, Key = "DefaultQuotationExpireDateDays", Description = "Número de dias de validade da cotação por defeito", Value = "15" });
            modelBuilder.Entity<Models.Database.Setting>().HasData(new Models.Database.Setting() { Id = 3, Key = "PartialPaymentMinimumValue", Description = "Valor mínimo da reserva para disponibilizar pagamento de sinal", Value = "333.33" });
            modelBuilder.Entity<Models.Database.Setting>().HasData(new Models.Database.Setting() { Id = 4, Key = "PartialPaymentDefaultPercentage", Description = "Percentagem do sinal", Value = "30" });

            // Translatable Settings
            modelBuilder.Entity<Models.Database.TranslatableSetting>().HasData(new Models.Database.TranslatableSetting() { Id = 1, Code = Enums.TranslatableSettings.TermsAndConditions });
            modelBuilder.Entity<Models.Database.TranslatableSetting>().HasData(new Models.Database.TranslatableSetting() { Id = 2, Code = Enums.TranslatableSettings.PrivacyPolicy });
            modelBuilder.Entity<Models.Database.TranslatableSetting>().HasData(new Models.Database.TranslatableSetting() { Id = 3, Code = Enums.TranslatableSettings.TermsAndConditionsVisaMastercard });

            // Languages
            Dictionary<int, string> languages = new Dictionary<int, string>()
            {
                {(int)Enums.Languages.Portuguese, "Português,pt,pt,https://flagcdn.com/pt.svg"},
                {(int)Enums.Languages.English, "English,en,uk,https://flagcdn.com/gb.svg"},
                {(int)Enums.Languages.French, "Français,fr,fr,https://flagcdn.com/fr.svg"},
            };

            foreach (KeyValuePair<int, string> language in languages)
            {
                string[] languageData = language.Value.Split(',');
                modelBuilder.Entity<dCore.MultiLanguage.Models.Language>().HasData(new dCore.MultiLanguage.Models.Language { Id = language.Key, Name = languageData[0], Code = languageData[1], CountryCode = languageData[2], FlagUrl = languageData[3] });
            }

            // Generic Texts
            List<dCore.MultiLanguage.Models.GenericText> genericTexts = new List<dCore.MultiLanguage.Models.GenericText>()
            {
                new() { Id = 1, Key = "Menu.Home"},
                new() { Id = 2, Key = "Menu.Passageiros"},
                new() { Id = 3, Key = "Menu.Comerciais"},
                new() { Id = 4, Key = "Menu.Renting"},
                new() { Id = 5, Key = "Menu.Ofertas"},
                new() { Id = 6, Key = "Menu.Blog"},
                new() { Id = 7, Key = "Menu.Contactos"},
                new() { Id = 8, Key = "Footer.Coluna1.Titulo" },
                new() { Id = 9, Key = "Footer.Coluna2.Titulo" },
                new() { Id = 10, Key = "Footer.Coluna3.Titulo" },
                new() { Id = 11, Key = "Footer.Coluna4.Titulo" },
                new() { Id = 12, Key = "Footer.RedesSociais.Titulo" },
                new() { Id = 13, Key = "Footer.SobreNos" },
                new() { Id = 14, Key = "Footer.NossaEquipa" },
                new() { Id = 15, Key = "Footer.GuiaDeAluguer" },
                new() { Id = 16, Key = "Footer.TermosECondicoes" },
                new() { Id = 17, Key = "Footer.PoliticaDePrivacidade" },
                new() { Id = 18, Key = "Footer.PerguntasERespostas" },
                new() { Id = 19, Key = "Footer.HorarioDeTrabalho" },
                new() { Id = 20, Key = "Footer.Balcoes" },
                new() { Id = 21, Key = "Footer.Queixas" },
                new() { Id = 22, Key = "Header.Registo" },
                new() { Id = 23, Key = "Header.Entrar" },
                new() { Id = 24, Key = "Home.Pesquisa.Recolha" },
                new() { Id = 25, Key = "Home.Pesquisa.Devolucao" },
                new() { Id = 26, Key = "Home.Pesquisa.Botao" },
                new() { Id = 27, Key = "Home.Campanhas.Titulo" },
                new() { Id = 28, Key = "Home.Campanhas.Subtitulo" },
                new() { Id = 29, Key = "Home.Processo.Titulo" },
                new() { Id = 30, Key = "Home.Processo.Subtitulo" },
                new() { Id = 31, Key = "Home.Factos.Titulo" },
                new() { Id = 32, Key = "Home.Factos.Subtitulo" },
                new() { Id = 33, Key = "Home.MaisAlugados.Titulo" },
                new() { Id = 34, Key = "Home.MaisAlugados.Subtitulo" },
                new() { Id = 35, Key = "Home.RazoesParaAlugarConnosco.Titulo" },
                new() { Id = 36, Key = "Home.RazoesParaAlugarConnosco.Subtitulo" },
                new() { Id = 37, Key = "Home.Testemunhos.Titulo" },
                new() { Id = 38, Key = "Home.Testemunhos.Subtitulo" },
                new() { Id = 39, Key = "Pesquisa.Filtros.Categoria" },
                new() { Id = 40, Key = "Pesquisa.Filtros.Combustivel" },
                new() { Id = 41, Key = "Pesquisa.Filtros.Transmissao" },
                new() { Id = 42, Key = "Pesquisa.Filtros.Botao" },
                new() { Id = 43, Key = "Pesquisa.Filtros.LimparFiltros" },
                new() { Id = 44, Key = "Pesquisa.Segmento.OuSimilar" },
                new() { Id = 45, Key = "Pesquisa.Segmento.Preco.Dia" },
                new() { Id = 46, Key = "Pesquisa.Segmento.Botao" },
                new() { Id = 47, Key = "Segmento.Detalhe.Sidebar.Titulo" },
                new() { Id = 48, Key = "Segmento.Detalhe.Sidebar.LocalRecolha" },
                new() { Id = 49, Key = "Segmento.Detalhe.Sidebar.LocalDevolucao" },
                new() { Id = 50, Key = "Segmento.Detalhe.Sidebar.DataRecolha" },
                new() { Id = 51, Key = "Segmento.Detalhe.Sidebar.DataDevolucao" },
                new() { Id = 52, Key = "Segmento.Detalhe.Sidebar.BotaoReservar" },
                new() { Id = 53, Key = "Segmento.Detalhe.Caracteristicas.Titulo" },
                new() { Id = 54, Key = "Segmento.Detalhe.Extras.Titulo" },
                new() { Id = 55, Key = "Segmento.Detalhe.Seguro.Titulo" },
                new() { Id = 56, Key = "Blog.Lista.Artigo.Botao" },
                new() { Id = 57, Key = "Contactos.Escritorios.Titulo" },
                new() { Id = 58, Key = "Contactos.Formulario.Titulo" },
                new() { Id = 59, Key = "Contactos.Formulario.Nome" },
                new() { Id = 60, Key = "Contactos.Formulario.Email" },
                new() { Id = 61, Key = "Contactos.Formulario.Telefone" },
                new() { Id = 62, Key = "Contactos.Formulario.Texto" },
                new() { Id = 63, Key = "Contactos.Formulario.Botao" },
                new() { Id = 64, Key = "PerguntasERespostas.Titulo" },
                new() { Id = 65, Key = "PerguntasERespostas.Subtitulo" },
                new() { Id = 66, Key = "Pesquisa.Titulo" },
                new() { Id = 67, Key = "Pesquisa.Filtros.Lugares" },
                new() { Id = 68, Key = "Contactos.Escritorios.Telefone" },
                new() { Id = 69, Key = "Contactos.Escritorios.Email" },
                new() { Id = 70, Key = "Contactos.Escritorios.Localizacao" },
                new() { Id = 71, Key = "Contactos.Escritorios.Horario" },
                new() { Id = 72, Key = "Contactos.Escritorios.Horario.SegundaASexta" },
                new() { Id = 73, Key = "Contactos.Escritorios.Horario.Sabado" },
                new() { Id = 74, Key = "Contactos.Escritorios.Localizacao.Abrir" },
                new() { Id = 75, Key = "Home.Processo.Botao" },
                new() { Id = 76, Key = "Home.MaisAlugados.Botao" },
                new() { Id = 77, Key = "Reserva.Confirmacao.Resumo.Titulo" },
                new() { Id = 78, Key = "Pesquisa.Filtros.Recolha" },
                new() { Id = 79, Key = "Pesquisa.Filtros.Devolucao" },
                new() { Id = 80, Key = "Reserva.Confirmacao.Resumo.Total" },
                new() { Id = 81, Key = "Pesquisa.Segmento.Preco.Total" },
                new() { Id = 82, Key = "Reserva.Confirmacao.Resumo.Segmento.OuSimilar" },
                new() { Id = 83, Key = "Pesquisa.Segmento.SemResultados" },
                new() { Id = 84, Key = "Blog.Lista.Sidebar.Categorias.Titulo" },
                new() { Id = 85, Key = "Blog.Lista.Sidebar.Tags.Titulo" },
                new() { Id = 86, Key = "Blog.Lista.Sidebar.MaisVistos.Titulo" },
                new() { Id = 87, Key = "Ficheiro.GuiaAluguer.Nome" },
                new() { Id = 88, Key = "Ofertas.Lista.Campanha.Botao" },
                new() { Id = 89, Key = "Ofertas.Lista.Campanha.Ate" },
                new() { Id = 90, Key = "Segmento.Detalhe.Sidebar.ValorVeiculo" },
                new() { Id = 91, Key = "Segmento.Detalhe.Sidebar.ValorExtras" },
                new() { Id = 92, Key = "Segmento.Detalhe.Sidebar.ValorSeguro" },
                new() { Id = 93, Key = "Segmento.Detalhe.Sidebar.ValorTotal" },
                new() { Id = 94, Key = "Segmento.Detalhe.Caracteristicas.Transmissao" },
                new() { Id = 95, Key = "Segmento.Detalhe.Caracteristicas.Combustivel" },
                new() { Id = 96, Key = "Segmento.Detalhe.Caracteristicas.Assentos" },
                new() { Id = 97, Key = "Segmento.Detalhe.Campanhas.Titulo" },
                new() { Id = 98, Key = "Segmento.Detalhe.Seguro.Franquia" },
                new() { Id = 99, Key = "Reserva.Titulo" },
                new() { Id = 100, Key = "Reserva.Passo" },
                new() { Id = 101, Key = "Reserva.Detalhe.Titulo" },
                new() { Id = 102, Key = "Reserva.Pagamento.Titulo" },
                new() { Id = 103, Key = "Reserva.Detalhe.Recolha" },
                new() { Id = 104, Key = "Reserva.Detalhe.Devolucao" },
                new() { Id = 105, Key = "Reserva.Detalhe.Segmento" },
                new() { Id = 106, Key = "Reserva.Detalhe.Extras" },
                new() { Id = 107, Key = "Reserva.Detalhe.Seguro" },
                new() { Id = 108, Key = "Reserva.Detalhe.Total" },
                new() { Id = 109, Key = "Reserva.Detalhe.DadosCondutor.Titulo" },
                new() { Id = 110, Key = "Reserva.Detalhe.DadosCondutor.Nome" },
                new() { Id = 111, Key = "Reserva.Detalhe.DadosCondutor.Email" },
                new() { Id = 112, Key = "Reserva.Detalhe.DadosCondutor.Telefone" },
                new() { Id = 113, Key = "Reserva.Detalhe.DadosFaturacao.Morada" },
                new() { Id = 114, Key = "Reserva.Detalhe.DadosFaturacao.CodigoPostal" },
                new() { Id = 115, Key = "Reserva.Detalhe.DadosFaturacao.LocalidadePostal" },
                new() { Id = 116, Key = "Reserva.Detalhe.DadosOutros.NumeroVoo" },
                new() { Id = 117, Key = "Reserva.Detalhe.DadosCondutor.DataNascimento" },
                new() { Id = 118, Key = "Reserva.Detalhe.DadosOutros.Comentarios" },
                new() { Id = 119, Key = "Reserva.Detalhe.DadosCondutor.NumeroIdentificacao" },
                new() { Id = 120, Key = "Reserva.Detalhe.DadosFaturacao.NumeroContribuinte" },
                new() { Id = 121, Key = "Reserva.Detalhe.DadosCondutor.NumeroCartaConducao" },
                new() { Id = 122, Key = "Reserva.Detalhe.DadosCondutor.DataValidadeCartaConducao" },
                new() { Id = 123, Key = "Reserva.Detalhe.DadosCondutor.PaisTelefone" },
                new() { Id = 124, Key = "Reserva.Detalhe.DadosCondutor.PaisIdentificacao" },
                new() { Id = 125, Key = "Reserva.Detalhe.DadosCondutor.PaisCartaConducao" },
                new() { Id = 126, Key = "Reserva.Detalhe.DadosFaturacao.Titulo" },
                new() { Id = 127, Key = "Reserva.Detalhe.DadosFaturacao.Nome" },
                new() { Id = 128, Key = "Reserva.Detalhe.DadosFaturacao.Email" },
                new() { Id = 129, Key = "Reserva.Detalhe.DadosFaturacao.PaisTelefone" },
                new() { Id = 130, Key = "Reserva.Detalhe.DadosFaturacao.Telefone" },
                new() { Id = 131, Key = "Reserva.Detalhe.DadosFaturacao.Pais" },
                new() { Id = 132, Key = "Segmento.Detalhe.Sidebar.ValorVeiculo.Dias" },
                new() { Id = 133, Key = "Segmento.Detalhe.Extras.Dia" },
                new() { Id = 134, Key = "Pesquisa.Filtros.Campanhas" },
                new() { Id = 135, Key = "Segmento.Detalhe.Titulo.OuSimilar" },
                new() { Id = 136, Key = "Reserva.Detalhe.Seguro.Franquia" },
                new() { Id = 137, Key = "Reserva.Detalhe.Segmento.OuSimilar" },
                new() { Id = 138, Key = "Reserva.Detalhe.Segmento.Dias" },
                new() { Id = 139, Key = "Reserva.Detalhe.DadosOutros.Titulo" },
                new() { Id = 140, Key = "Reserva.Detalhe.Botao" },
                new() { Id = 141, Key = "Reserva.Detalhe.DadosCondutor.DataEmissaoCartaConducao" },
                new() { Id = 142, Key = "Reserva.Detalhe.DadosCondutoresExtra.Titulo" },
                new() { Id = 143, Key = "Reserva.Detalhe.DadosCondutoresExtra.Nome" },
                new() { Id = 144, Key = "Reserva.Detalhe.DadosCondutoresExtra.PaisCartaConducao" },
                new() { Id = 145, Key = "Reserva.Detalhe.DadosCondutoresExtra.NumeroCartaConducao" },
                new() { Id = 146, Key = "Reserva.Detalhe.DadosCondutoresExtra.DataEmissaoCartaConducao" },
                new() { Id = 147, Key = "Reserva.Detalhe.DadosCondutoresExtra.DataValidadeCartaConducao" },
                new() { Id = 148, Key = "Reserva.Detalhe.DadosCondutoresExtra.DataNascimento" },
                new() { Id = 149, Key = "Reserva.Confirmacao.Resumo.Segmento" },
                new() { Id = 150, Key = "Reserva.Confirmacao.Resumo.Extras" },
                new() { Id = 151, Key = "Reserva.Confirmacao.Resumo.Recolha" },
                new() { Id = 152, Key = "Reserva.Confirmacao.Resumo.Devolucao" },
                new() { Id = 153, Key = "Reserva.Confirmacao.Resumo.Seguro" },
                new() { Id = 154, Key = "Reserva.Confirmacao.Resumo.Pagamento" },
                new() { Id = 155, Key = "Reserva.Confirmacao.Resumo.Informacao" },
                new() { Id = 156, Key = "Reserva.Confirmacao.Titulo" },
                new() { Id = 157, Key = "Reserva.Confirmacao.Resumo.Seguro.Franquia" },
                new() { Id = 158, Key = "Reserva.Confirmacao.Resumo.Segmento.Dias" },
                new() { Id = 159, Key = "Reserva.Pagamento.Valor.Titulo" },
                new() { Id = 160, Key = "Reserva.Pagamento.EscolhaMetodo.Titulo" },
                new() { Id = 161, Key = "Reserva.Pagamento.Botao" },
                new() { Id = 162, Key = $"MetodoPagamento.{Enums.PaymentTypes.BankTransfer.ToString()}" },
                new() { Id = 163, Key = $"MetodoPagamento.{Enums.PaymentTypes.CreditCard.ToString()}" },
                new() { Id = 164, Key = $"MetodoPagamento.{Enums.PaymentTypes.MBReference.ToString()}" },
                new() { Id = 165, Key = $"MetodoPagamento.{Enums.PaymentTypes.MBWay.ToString()}" },
                new() { Id = 166, Key = $"MetodoPagamento.{Enums.PaymentTypes.Paypal.ToString()}" },
                new() { Id = 167, Key = "Reserva.Confirmacao.Resumo.Notas" },
                new() { Id = 168, Key = "Reserva.Confirmacao.Resumo.Notas.Texto" },
                new() { Id = 169, Key = "Placeholder.Data" },
                new() { Id = 170, Key = "Placeholder.Hora" },
                new() { Id = 171, Key = "Pesquisa.Filtros.RecolhaDevolucao.AvisoDataHoraAjustada" },
                new() { Id = 172, Key = "Segmento.Detalhe.Sidebar.AvisoDataHoraAjustada" },
                new() { Id = 173, Key = "Contactos.Escritorios.Horario.Domingo" },
                new() { Id = 174, Key = "Login.Titulo" },
                new() { Id = 175, Key = "Login.Email" },
                new() { Id = 176, Key = "Login.Password" },
                new() { Id = 177, Key = "Login.Botao" },
                new() { Id = 178, Key = "Login.Registar.Texto" },
                new() { Id = 179, Key = "Login.Registar.Link" },
                new() { Id = 180, Key = "Reserva.Pagamento.TransferenciaBancaria.Texto" },
                new() { Id = 181, Key = "Reserva.Pagamento.CartaoCredito.Texto" },
                new() { Id = 182, Key = "Reserva.Pagamento.ReferenciaMultibanco.Texto" },
                new() { Id = 183, Key = "Footer.TermosECondicoesVisaMastercard" },
                new() { Id = 184, Key = "Reserva.Pagamento.MbWay.Texto" },
                new() { Id = 185, Key = "EmailRegistoReserva.Pagamento.CartaoCredito" },
                new() { Id = 186, Key = "EmailRegistoReserva.Pagamento.MBWay" },
                new() { Id = 187, Key = "EmailRegistoReserva.Pagamento.TransferenciaBancaria" },
                new() { Id = 188, Key = "EmailRegistoReserva.Pagamento.Multibanco" },
                new() { Id = 189, Key = "EmailRegistoReserva.Pagamento.Multibanco.Entidade" },
                new() { Id = 190, Key = "EmailRegistoReserva.Pagamento.Multibanco.Referencia" },
                new() { Id = 191, Key = "EmailRegistoReserva.Pagamento.Multibanco.Valor" },
                new() { Id = 192, Key = "Reserva.Confirmacao.Resumo.Pagamento.TransferenciaBancaria" },
                new() { Id = 193, Key = "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Entidade" },
                new() { Id = 194, Key = "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Referencia" },
                new() { Id = 195, Key = "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Valor" },
                new() { Id = 196, Key = "Segmento.Detalhe.Sidebar.CodigoDesconto" },
                new() { Id = 197, Key = "Segmento.Detalhe.Sidebar.CodigoDesconto.Botao" },
                new() { Id = 198, Key = "Segmento.Detalhe.Sidebar.CodigoDesconto.Invalido" },
                new() { Id = 199, Key = "Email.CoberturasIncluidas" },
                new() { Id = 200, Key = "Email.EquipaAMRent" },
                new() { Id = 201, Key = "Cotacao.Pagamento.Titulo" },
                new() { Id = 202, Key = "Cotacao.Pagamento.Botao" },
                new() { Id = 203, Key = "Registo.Titulo" },
                new() { Id = 204, Key = "Registo.Email" },
                new() { Id = 205, Key = "Registo.Password" },
                new() { Id = 206, Key = "Registo.Botao" },
                new() { Id = 207, Key = "Registo.Login.Texto" },
                new() { Id = 208, Key = "Registo.Login.Link" },
                new() { Id = 209, Key = "Header.MinhaConta" },
                new() { Id = 210, Key = "Header.Logout" },
                new() { Id = 211, Key = "Login.EsqueceuPassword" },
                new() { Id = 212, Key = "PasswordEsquecida.Titulo" },
                new() { Id = 213, Key = "PasswordEsquecida.Instrucoes" },
                new() { Id = 214, Key = "PasswordEsquecida.Email" },
                new() { Id = 215, Key = "PasswordEsquecida.Botao" },
                new() { Id = 216, Key = "EmailRegistoReserva.Pagamento.PosPagamento" },
                new() { Id = 217, Key = "Segmento.Detalhe.Caracteristicas.ComprimentoCarga" },
                new() { Id = 218, Key = "Segmento.Detalhe.Caracteristicas.LarguraCarga" },
                new() { Id = 219, Key = "Segmento.Detalhe.Caracteristicas.AlturaCarga" },
                new() { Id = 220, Key = $"MetodoPagamento.{Enums.PaymentTypes.Cash.ToString()}" },
                new() { Id = 221, Key = "Email.Sumario.PagamentoParcialAntecipado" },
                new() { Id = 222, Key = "Email.Sumario.DetalheCoberturas.Titulo" },
                new() { Id = 223, Key = "MinhaConta.Perfil.Titulo" },
                new() { Id = 224, Key = "MinhaConta.Perfil.DadosPessoais.Titulo" },
                new() { Id = 225, Key = "MinhaConta.Perfil.DadosFaturacao.Titulo" },
                new() { Id = 226, Key = "MinhaConta.Perfil.Botao" },
                new() { Id = 227, Key = "MinhaConta.Reservas.Titulo" },
                new() { Id = 228, Key = "MinhaConta.Reservas.SemResultados" },
                new() { Id = 229, Key = "Reserva.Pagamento.Paypal.Texto" },
                new() { Id = 230, Key = "EmailRegistoReserva.Pagamento.Paypal" },
                new() { Id = 231, Key = "Reserva.Detalhe.DadosCondutor.TipoIdentificacao" },
                new() { Id = 232, Key = "Reserva.Detalhe.DadosCondutor.TipoIdentificacao.CartaoCidadaoBilheteIdentidade" },
                new() { Id = 233, Key = "Reserva.Detalhe.DadosCondutor.TipoIdentificacao.Passaporte" },
                new() { Id = 234, Key = "Reserva.Detalhe.DadosCondutor.NumeroContribuinte" },
                new() { Id = 235, Key = "Reserva.Detalhe.Consentimentos.Titulo" },
                new() { Id = 236, Key = "Documento.Cotacao.Numero" },
                new() { Id = 237, Key = "Documento.Cotacao.Data" },
                new() { Id = 238, Key = "Documento.Cotacao.Validade" },
                new() { Id = 239, Key = "Documento.Cotacao.ResumoDeCustos" },
                new() { Id = 240, Key = "Documento.Reserva.Numero" },
                new() { Id = 241, Key = "Documento.Reserva.Data" },
                new() { Id = 242, Key = "Documento.Reserva.Validade" },
                new() { Id = 243, Key = "Documento.Reserva.ResumoDeCustos" },
                new() { Id = 244, Key = "Segmento.Detalhe.Sidebar.BotaoPedirCotacao" },
                new() { Id = 245, Key = "PedidoCotacao.Detalhe.Titulo" },
                new() { Id = 246, Key = "PedidoCotacao.Detalhe.Botao" },
                new() { Id = 247, Key = "PedidoCotacao.Confirmacao.Resumo.Titulo" },
                new() { Id = 248, Key = "PedidoCotacao.Confirmacao.Resumo.Notas" },
                new() { Id = 249, Key = "PedidoCotacao.Confirmacao.Resumo.Notas.Texto" },
                new() { Id = 250, Key = "Extra.Incluido" },
                new() { Id = 251, Key = "Documento.Cotacao.NomeCliente" },
                new() { Id = 252, Key = "Documento.Reserva.NomeCliente" },
            };

            List<dCore.MultiLanguage.Models.GenericTextTranslation> genericTextsTranslations = new List<dCore.MultiLanguage.Models.GenericTextTranslation>()
            {
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 1, Value = "Home" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 2, Value = "Passageiros" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 3, Value = "Comerciais" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 4, Value = "Renting" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 5, Value = "Ofertas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 6, Value = "Blog" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 7, Value = "Contactos" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 8, Value = "Sobre nós" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 9, Value = "Mais" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 10, Value = "Ligações rápidas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 11, Value = "Informação de contacto" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 12, Value = "Conecte-se connosco" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 13, Value = "Sobre nós" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 14, Value = "A nossa equipa" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 15, Value = "Guia de aluguer" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 16, Value = "Termos e condições" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 17, Value = "Política de privacidade" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 18, Value = "Perguntas e respostas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 19, Value = "Horário de trabalho" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 20, Value = "Balcões" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 21, Value = "Queixas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 22, Value = "Registo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 23, Value = "Entrar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 24, Value = "Recolha" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 25, Value = "Devolução" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 26, Value = "Procurar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 27, Value = "Campanhas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 28, Value = "Campanhas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 29, Value = "Processo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 30, Value = "Processo.Subtítulo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 31, Value = "Factos" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 32, Value = "Factos.Subtítulo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 33, Value = "Mais alugados" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 34, Value = "MaisAlugados.Subtítulo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 35, Value = "Razões para alugar connosco" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 36, Value = "RazõesParaAlugarconnosco.Subtítulo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 37, Value = "Testemunhos" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 38, Value = "Testemunhos.Subtítulo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 39, Value = "Categoria" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 40, Value = "Combustível" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 41, Value = "Transmissão" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 42, Value = "Filtrar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 43, Value = "Limpar filtros" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 44, Value = "Ou similar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 45, Value = "Dia" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 46, Value = "Alugar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 47, Value = "Recolha e Devolução" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 48, Value = "Local de recolha" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 49, Value = "Local de devolução" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 50, Value = "Data de recolha" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 51, Value = "Data de devolucao" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 52, Value = "Alugar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 53, Value = "Características" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 54, Value = "Extras" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 55, Value = "Seguro" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 56, Value = "Ler mais" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 57, Value = "Escritórios" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 58, Value = "Contacte-nos" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 59, Value = "Nome" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 60, Value = "Email" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 61, Value = "Telefone" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 62, Value = "Texto" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 63, Value = "Enviar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 64, Value = "Perguntas e respostas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 65, Value = "" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 66, Value = "Pesquisa" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 67, Value = "Lugares" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 68, Value = "Telefone" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 69, Value = "Email" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 70, Value = "Localização" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 71, Value = "Horário" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 72, Value = "Seg-Sex" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 73, Value = "Sábado" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 74, Value = "Abrir no Google Maps" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 75, Value = "Pedir Proposta" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 76, Value = "Ver todos" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 77, Value = "Confirmação de reserva" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 78, Value = "Recolha" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 79, Value = "Devolução" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 80, Value = "Total" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 81, Value = "Total" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 82, Value = "ou similar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 83, Value = "Sem Resultados" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 84, Value = "Categorias" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 85, Value = "Tags" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 86, Value = "Mais vistos" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 87, Value = "Guia de aluguer" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 88, Value = "Mais" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 89, Value = "Até" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 90, Value = "Perído de aluguer" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 91, Value = "Extras" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 92, Value = "Protecção" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 93, Value = "Total" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 94, Value = "Transmissão" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 95, Value = "Combustível" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 96, Value = "Assentos" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 97, Value = "Campanhas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 98, Value = "Franquia" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 99, Value = "Reserva" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 100, Value = "Passo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 101, Value = "Detalhe" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 102, Value = "Pagamento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 103, Value = "Recolha" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 104, Value = "Devolução" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 105, Value = "Segmento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 106, Value = "Extras" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 107, Value = "Protecção" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 108, Value = "Total a pagar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 109, Value = "Dados do condutor" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 110, Value = "Nome" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 111, Value = "Email" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 112, Value = "Telefone" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 113, Value = "Morada" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 114, Value = "Cédigo postal" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 115, Value = "Localidade postal" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 116, Value = "Numero de Voo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 117, Value = "Data de nascimento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 118, Value = "Comentários" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 119, Value = "Nº de identificação" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 120, Value = "Nº de contribuinte" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 121, Value = "Nº carta de condução" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 122, Value = "Validade da carta de condução" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 123, Value = "Indicativo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 124, Value = "País ID" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 125, Value = "País" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 126, Value = "Dados de faturação" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 127, Value = "Nome" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 128, Value = "Email" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 129, Value = "Indicativo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 130, Value = "Telefone" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 131, Value = "País" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 132, Value = "dias" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 133, Value = "dia" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 134, Value = "Campanhas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 135, Value = "ou similar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 136, Value = "Franquia" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 137, Value = "ou similar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 138, Value = "dias" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 139, Value = "Outros dados" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 140, Value = "Ir para pagamento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 141, Value = "Data de emissão" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 142, Value = "Dados do condutor Extra" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 143, Value = "Nome" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 144, Value = "Carta de condução" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 145, Value = "Número" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 146, Value = "Data de emissão" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 147, Value = "Validade" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 148, Value = "Data de nascimento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 149, Value = "Segmento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 150, Value = "Extras" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 151, Value = "Recolha" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 152, Value = "Devolução" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 153, Value = "Seguro" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 154, Value = "Pagamento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 155, Value = "Informação" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 156, Value = "Confirmação de reserva" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 157, Value = "Franquia" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 158, Value = "dias" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 159, Value = "Valor" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 160, Value = "Escolha o método de pagamento. (Só transferência disponível. Mais brevemente)" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 161, Value = "Pagar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 162, Value = "Transferência bancária" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 163, Value = "Cartão de crédito" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 164, Value = "Referência multibanco" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 165, Value = "MB Way" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 166, Value = "Paypal" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 167, Value = "Reserva.Confirmacao.Resumo.Notas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 168, Value = "Reserva.Confirmacao.Resumo.Notas.Texto" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 169, Value = "DD-MM-AAAA" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 170, Value = "HH:MM" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 171, Value = "AvisoDataHoraAjustada" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 172, Value = "AvisoDataHoraAjustada" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 173, Value = "Domingo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 174, Value = "Entrar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 175, Value = "Endereço de email" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 176, Value = "Palavra passe" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 177, Value = "Entrar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 178, Value = "Ainda não tem conta?" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 179, Value = "Registar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 180, Value = "Texto para pagamento por transferência" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 181, Value = "Texto para pagamento por cartão de crédito" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 182, Value = "Texto para pagamento por referência multibanco" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 183, Value = "Termos e condições de pagamento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 184, Value = "Número" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 185, Value = "EmailRegistoReserva.Pagamento.CartaoCredito" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 186, Value = "EmailRegistoReserva.Pagamento.MBWay" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 187, Value = "EmailRegistoReserva.Pagamento.TransferenciaBancaria" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 188, Value = "EmailRegistoReserva.Pagamento.Multibanco" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 189, Value = "EmailRegistoReserva.Pagamento.Multibanco.Entidade" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 190, Value = "EmailRegistoReserva.Pagamento.Multibanco.Referencia" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 191, Value = "EmailRegistoReserva.Pagamento.Multibanco.Valor" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 192, Value = "Reserva.Confirmacao.Resumo.Pagamento.TransferenciaBancaria" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 193, Value = "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Entidade" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 194, Value = "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Referencia" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 195, Value = "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Valor" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 196, Value = "Código de desconto" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 197, Value = "Aplicar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 198, Value = "Código de desconto inválido" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 199, Value = "Coberturas incluídas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 200, Value = "A equipa AMRent" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 201, Value = "Pagamento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 202, Value = "Pagar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 203, Value = "Registar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 204, Value = "Endereço de email" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 205, Value = "Palavra passe" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 206, Value = "Registar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 207, Value = "Já tem conta?" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 208, Value = "Login" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 209, Value = "My AMC" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 210, Value = "Sair" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 211, Value = "Esqueceu a password?" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 212, Value = "Recuperação de password" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 213, Value = "Diga-nos o seu email e enviaremos um link para redefinir a sua password." },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 214, Value = "Email" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 215, Value = "Submeter" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 216, Value = "EmailRegistoReserva.Pagamento.PosPagamento" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 217, Value = "ComprimentoCarga" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 218, Value = "LarguraCarga" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 219, Value = "AlturaCarga" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 220, Value = "Numerário" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 221, Value = "Pagamento parcial antecipado" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 222, Value = "Detalhe das coberturas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 223, Value = "Perfil" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 224, Value = "Dados pessoais" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 225, Value = "Dados de faturação" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 226, Value = "Gravar" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 227, Value = "Reservas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 228, Value = "Ainda não efectuou nenhuma reserva" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 229, Value = "Texto para pagamento por paypal" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 230, Value = "Reserva.Confirmacao.Resumo.Pagamento.TransferenciaBancaria" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 231, Value = "Tipo de identificacao" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 232, Value = "CC/BI" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 233, Value = "Passaporte" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 234, Value = "Nº contribuinte" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 235, Value = "Reserva.Detalhe.Consentimentos.Titulo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 236, Value = "Documento.Cotacao.Numero" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 237, Value = "Documento.Cotacao.Data" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 238, Value = "Documento.Cotacao.Validade" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 239, Value = "Documento.Cotacao.ResumoDeCustos" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 240, Value = "Documento.Reserva.Numero" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 241, Value = "Documento.Reserva.Data" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 242, Value = "Documento.Reserva.Validade" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 243, Value = "Documento.Reserva.ResumoDeCustos" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 244, Value = "Segmento.Detalhe.Sidebar.BotaoPedirCotacao" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 245, Value = "PedidoCotacao.Detalhe.Titulo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 246, Value = "PedidoCotacao.Detalhe.Botao" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 247, Value = "PedidoCotacao.Confirmacao.Resumo.Titulo" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 248, Value = "PedidoCotacao.Confirmacao.Resumo.Notas" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 249, Value = "PedidoCotacao.Confirmacao.Resumo.Notas.Texto" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 250, Value = "Extra.Incluido" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 251, Value = "Documento.Cotacao.NomeCliente" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, GenericTextId = 252, Value = "Documento.Reserva.NomeCliente" },
            };

            modelBuilder.Entity<dCore.MultiLanguage.Models.GenericText>().HasData(genericTexts);
            modelBuilder.Entity<dCore.MultiLanguage.Models.GenericTextTranslation>().HasData(genericTextsTranslations);

            // Processes
            List<Models.Database.Process> processes = new List<Models.Database.Process>()
            {
                new() {Id = 1, FontAwesomeIconCode = ""},
                new() {Id = 2, FontAwesomeIconCode = ""},
                new() {Id = 3, FontAwesomeIconCode = ""}
            };
            List<Models.Database.ProcessTranslation> processTranslations = new List<Models.Database.ProcessTranslation>()
            {
                new() { LanguageId = (int)Enums.Languages.Portuguese, ProcessId = 1, Title = "Processo 1", Text = "" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, ProcessId = 2, Title = "Processo 2", Text = "" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, ProcessId = 3, Title = "Processo 3", Text = "" }
            };

            modelBuilder.Entity<Models.Database.Process>().HasData(processes);
            modelBuilder.Entity<Models.Database.ProcessTranslation>().HasData(processTranslations);

            // Reasons to choose us
            List<Models.Database.ReasonToChooseUs> reasonsToChooseUs = new List<Models.Database.ReasonToChooseUs>()
            {
                new() {Id = 1, FontAwesomeIconCode = ""},
                new() {Id = 2, FontAwesomeIconCode = ""},
                new() {Id = 3, FontAwesomeIconCode = ""},
                new() {Id = 4, FontAwesomeIconCode = ""}
            };
            List<Models.Database.ReasonToChooseUsTranslation> reasonToChooseUsTranslations = new List<Models.Database.ReasonToChooseUsTranslation>()
            {
                new() { LanguageId = (int)Enums.Languages.Portuguese, ReasonToChooseUsId = 1, Title = "Razão 1", Text = "" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, ReasonToChooseUsId = 2, Title = "Razão 2", Text = "" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, ReasonToChooseUsId = 3, Title = "Razão 3", Text = "" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, ReasonToChooseUsId = 4, Title = "Razão 4", Text = "" }
            };

            modelBuilder.Entity<Models.Database.ReasonToChooseUs>().HasData(reasonsToChooseUs);
            modelBuilder.Entity<Models.Database.ReasonToChooseUsTranslation>().HasData(reasonToChooseUsTranslations);

            // Facts
            List<Models.Database.Fact> facts = new List<Models.Database.Fact>()
            {
                new() {Id = 1, Number = 100, FontAwesomeIconCode = ""},
                new() {Id = 2, Number = 100, FontAwesomeIconCode = ""},
                new() {Id = 3, Number = 100, FontAwesomeIconCode = ""},
                new() {Id = 4, Number = 100, FontAwesomeIconCode = ""}
            };
            List<Models.Database.FactTranslation> factTranslations = new List<Models.Database.FactTranslation>()
            {
                new() { LanguageId = (int)Enums.Languages.Portuguese, FactId = 1, Title = "Facto 1" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, FactId = 2, Title = "Facto 2" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, FactId = 3, Title = "Facto 3" },
                new() { LanguageId = (int)Enums.Languages.Portuguese, FactId = 4, Title = "Facto 4" }
            };

            modelBuilder.Entity<Models.Database.Fact>().HasData(facts);
            modelBuilder.Entity<Models.Database.FactTranslation>().HasData(factTranslations);

            // Emails
            List<Models.Database.EmailContent> emailContents = new();
            List<Models.Database.EmailContentTranslation> emailContentTranslations = new();
            foreach (Enums.EmailContentTypes emailContentType in (Enums.EmailContentTypes[])Enum.GetValues(typeof(Enums.EmailContentTypes)))
            {
                emailContents.Add(new() { Id = (int)emailContentType, Type = emailContentType, SendQuotationReservationSummaryPdf = false });
                emailContentTranslations.Add(new() { LanguageId = (int)Enums.Languages.Portuguese, EmailContentId = (int)emailContentType, Subject = "", Text = "" });
            }

            modelBuilder.Entity<Models.Database.EmailContent>().HasData(emailContents);
            modelBuilder.Entity<Models.Database.EmailContentTranslation>().HasData(emailContentTranslations);

            // Season Categories
            List<Models.Database.SeasonCategory> seasonCategories = new List<Models.Database.SeasonCategory>()
            {
                new() {Id = 1, Name = "Verão"},
                new() {Id = 2, Name = "Inverno"},
            };

            modelBuilder.Entity<Models.Database.SeasonCategory>().HasData(seasonCategories);

            // Insurance Levels
            List<Models.Database.InsuranceLevel> insuranceLevels = new List<Models.Database.InsuranceLevel>()
            {
                new() {Id = 1},
                new() {Id = 2},
                new() {Id = 3}
            };
            List<Models.Database.InsuranceLevelTranslation> insuranceLevelTranslations = new List<Models.Database.InsuranceLevelTranslation>()
            {
                new() { InsuranceLevelId = 1, LanguageId = (int)Enums.Languages.Portuguese, Name = "Nível 1", Included = "", Excluded = "" },
                new() { InsuranceLevelId = 2, LanguageId = (int)Enums.Languages.Portuguese, Name = "Nível 2", Included = "", Excluded = "" },
                new() { InsuranceLevelId = 3, LanguageId = (int)Enums.Languages.Portuguese, Name = "Nível 3", Included = "", Excluded = "" }
            };

            modelBuilder.Entity<Models.Database.InsuranceLevel>().HasData(insuranceLevels);
            modelBuilder.Entity<Models.Database.InsuranceLevelTranslation>().HasData(insuranceLevelTranslations);

            // Pickup/Return Locations
            List<Models.Database.PickupReturnLocation> pickupReturnLocation = new List<Models.Database.PickupReturnLocation>()
            {
                new() {Id = -1, IsWorkingOffice = false, IsSelectedByDefault = false, IsAlwaysAvailableForPickupAndReturn = false, MinimumAnticipationMinutes = 2880}
            };
            List<Models.Database.PickupReturnLocationTranslation> pickupReturnLocationTranslations = new List<Models.Database.PickupReturnLocationTranslation>()
            {
                new() { PickupReturnLocationId = -1, LanguageId = (int)Enums.Languages.Portuguese, Name = "Outro" },
                new() { PickupReturnLocationId = -1, LanguageId = (int)Enums.Languages.English, Name = "Other" },
                new() { PickupReturnLocationId = -1, LanguageId = (int)Enums.Languages.French, Name = "Autre" }
            };
            List<Models.Database.PickupReturnLocationDayOfWeekSchedule> pickupReturnLocationDayOfWeekSchedules = new List<Models.Database.PickupReturnLocationDayOfWeekSchedule>()
            {
                new() { PickupReturnLocationId = -1, DayOfWeek = Enums.DaysOfWeek.Monday, OpeningTime = new TimeSpan(0), ClosingTime = new TimeSpan(23, 59, 0), IsClosed = false },
                new() { PickupReturnLocationId = -1, DayOfWeek = Enums.DaysOfWeek.Tuesday, OpeningTime = new TimeSpan(0), ClosingTime = new TimeSpan(23, 59, 0), IsClosed = false },
                new() { PickupReturnLocationId = -1, DayOfWeek = Enums.DaysOfWeek.Wednesday, OpeningTime = new TimeSpan(0), ClosingTime = new TimeSpan(23, 59, 0), IsClosed = false },
                new() { PickupReturnLocationId = -1, DayOfWeek = Enums.DaysOfWeek.Thursday, OpeningTime = new TimeSpan(0), ClosingTime = new TimeSpan(23, 59, 0), IsClosed = false },
                new() { PickupReturnLocationId = -1, DayOfWeek = Enums.DaysOfWeek.Friday, OpeningTime = new TimeSpan(0), ClosingTime = new TimeSpan(23, 59, 0), IsClosed = false },
                new() { PickupReturnLocationId = -1, DayOfWeek = Enums.DaysOfWeek.Saturday, OpeningTime = new TimeSpan(0), ClosingTime = new TimeSpan(23, 59, 0), IsClosed = false },
                new() { PickupReturnLocationId = -1, DayOfWeek = Enums.DaysOfWeek.Sunday , OpeningTime = new TimeSpan(0), ClosingTime = new TimeSpan(23, 59, 0), IsClosed = false },
            };

            modelBuilder.Entity<Models.Database.PickupReturnLocation>().HasData(pickupReturnLocation);
            modelBuilder.Entity<Models.Database.PickupReturnLocationTranslation>().HasData(pickupReturnLocationTranslations);
            modelBuilder.Entity<Models.Database.PickupReturnLocationDayOfWeekSchedule>().HasData(pickupReturnLocationDayOfWeekSchedules);

            //// CustomerSources
            //modelBuilder.Entity<Models.Database.CustomerSource>().HasData(new Models.Database.CustomerSource() { Id = 1, Description = "Recomendação" });
            //modelBuilder.Entity<Models.Database.CustomerSource>().HasData(new Models.Database.CustomerSource() { Id = 2, Description = "Digital" });
            //modelBuilder.Entity<Models.Database.CustomerSource>().HasData(new Models.Database.CustomerSource() { Id = 3, Description = "Já é cliente" });

            // ReservationQuotationCancellationReason
            modelBuilder.Entity<Models.Database.ReservationQuotationCancellationReason>().HasData(new Models.Database.ReservationQuotationCancellationReason() { Id = 1, Description = "Datas alteradas" });
            modelBuilder.Entity<Models.Database.ReservationQuotationCancellationReason>().HasData(new Models.Database.ReservationQuotationCancellationReason() { Id = 2, Description = "Voo cancelado" });
            modelBuilder.Entity<Models.Database.ReservationQuotationCancellationReason>().HasData(new Models.Database.ReservationQuotationCancellationReason() { Id = 3, Description = "Preço elevado" });
            modelBuilder.Entity<Models.Database.ReservationQuotationCancellationReason>().HasData(new Models.Database.ReservationQuotationCancellationReason() { Id = 4, Description = "Pagamento recusado" });
            modelBuilder.Entity<Models.Database.ReservationQuotationCancellationReason>().HasData(new Models.Database.ReservationQuotationCancellationReason() { Id = 5, Description = "Sem cartão de crédito" });
            modelBuilder.Entity<Models.Database.ReservationQuotationCancellationReason>().HasData(new Models.Database.ReservationQuotationCancellationReason() { Id = 6, Description = "Reserva duplicada" });
            modelBuilder.Entity<Models.Database.ReservationQuotationCancellationReason>().HasData(new Models.Database.ReservationQuotationCancellationReason() { Id = 7, Description = "Planos alterados" });
            modelBuilder.Entity<Models.Database.ReservationQuotationCancellationReason>().HasData(new Models.Database.ReservationQuotationCancellationReason() { Id = 8, Description = "Não necessita de viatura" });
        }

        // ##### DBSets #####

        // Identity
        public DbSet<Models.Database.User> Users { get; set; }
        public DbSet<Models.Database.Role> Roles { get; set; }
        public DbSet<Models.Database.RoleUser> RolesUsers { get; set; }
        public DbSet<Models.Database.RolePermission> RolePermissions { get; set; }

        // Business
        public DbSet<Models.Database.About> About { get; set; }
        public DbSet<Models.Database.AboutTranslation> AboutTranslations { get; set; }
        public DbSet<Models.Database.BlogArticle> BlogArticles { get; set; }
        public DbSet<Models.Database.BlogArticleTranslation> BlogArticleTranslations { get; set; }
        public DbSet<Models.Database.BlogArticleCategory> BlogArticleCategories { get; set; }
        public DbSet<Models.Database.BlogArticleCategoryTranslation> BlogArticleCategoryTranslations { get; set; }
        public DbSet<Models.Database.Campaign> Campaigns { get; set; }
        public DbSet<Models.Database.CampaignCarSegment> CampaignCarSegments { get; set; }
        public DbSet<Models.Database.CampaignExtra> CampaignExtras { get; set; }
        public DbSet<Models.Database.CampaignTranslation> CampaignTranslations { get; set; }
        public DbSet<Models.Database.CarCategory> CarCategories { get; set; }
        public DbSet<Models.Database.CarCategoryTranslation> CarCategoryTranslations { get; set; }
        public DbSet<Models.Database.CarFuel> CarFuels { get; set; }
        public DbSet<Models.Database.CarFuelTranslation> CarFuelTranslations { get; set; }
        public DbSet<Models.Database.CarGearbox> CarGearboxes { get; set; }
        public DbSet<Models.Database.CarGearboxTranslation> CarGearboxTranslations { get; set; }
        public DbSet<Models.Database.CarSegment> CarSegments { get; set; }
        public DbSet<Models.Database.CarSegmentTranslation> CarSegmentTranslations { get; set; }
        public DbSet<Models.Database.Country> Countries { get; set; }
        public DbSet<Models.Database.CountryTranslation> CountryTranslations { get; set; }
        //public DbSet<Models.Database.CustomerSource> CustomerSources { get; set; }
        public DbSet<Models.Database.DataProtectionConsent> DataProtectionConsents { get; set; }
        public DbSet<Models.Database.DataProtectionConsentQuotation> DataProtectionConsentQuotations { get; set; }
        public DbSet<Models.Database.DataProtectionConsentReservation> DataProtectionConsentReservations { get; set; }
        public DbSet<Models.Database.DataProtectionConsentTranslation> DataProtectionConsentTranslations { get; set; }
        public DbSet<Models.Database.DataProtectionConsentUser> DataProtectionConsentsUsers { get; set; }
        public DbSet<Models.Database.EmailContent> EmailContents { get; set; }
        public DbSet<Models.Database.EmailContentTranslation> EmailContentTranslations { get; set; }
        public DbSet<Models.Database.Extra> Extras { get; set; }
        public DbSet<Models.Database.ExtraPriceByInsuranceLevel> ExtraPricesByInsuranceLevel { get; set; }
        public DbSet<Models.Database.ExtraTranslation> ExtraTranslations { get; set; }
        public DbSet<Models.Database.Fact> Facts { get; set; }
        public DbSet<Models.Database.FactTranslation> FactTranslations { get; set; }
        public DbSet<dCore.MultiLanguage.Models.GenericText> GenericTexts { get; set; }
        public DbSet<dCore.MultiLanguage.Models.GenericTextTranslation> GenericTextTranslations { get; set; }
        public DbSet<Models.Database.HomeBanner> HomeBanners { get; set; }
        public DbSet<Models.Database.HomeBannerTranslation> HomeBannerTranslations { get; set; }
        public DbSet<Models.Database.Insurance> Insurances { get; set; }
        public DbSet<Models.Database.InsuranceChange> InsuranceChanges { get; set; }
        public DbSet<Models.Database.InsuranceLevel> InsuranceLevels { get; set; }
        public DbSet<Models.Database.InsuranceLevelTranslation> InsuranceLevelTranslations { get; set; }
        public DbSet<Models.Database.InsurancePrice> InsurancePrices { get; set; }
        public DbSet<dCore.MultiLanguage.Models.Language> Languages { get; set; }
        public DbSet<Models.Database.OfficeLocation> OfficeLocations { get; set; }
        public DbSet<Models.Database.OfficeLocationTranslation> OfficeLocationTranslations { get; set; }
        public DbSet<Models.Database.PickupReturnLocation> PickupReturnLocations { get; set; }
        public DbSet<Models.Database.PickupReturnLocationDayOfWeekSchedule> PickupReturnLocationDayOfWeekSchedules { get; set; }
        public DbSet<Models.Database.PickupReturnLocationTax> PickupReturnLocationTaxes { get; set; }
        public DbSet<Models.Database.PickupReturnLocationTaxChange> PickupReturnLocationTaxChanges { get; set; }
        public DbSet<Models.Database.PickupReturnLocationTranslation> PickupReturnLocationTranslations { get; set; }
        public DbSet<Models.Database.PickupReturnTemporaryTax> PickupReturnTemporaryTaxes { get; set; }
        public DbSet<Models.Database.PickupReturnTemporaryTaxChange> PickupReturnTemporaryTaxChanges { get; set; }
        public DbSet<Models.Database.Price> Prices { get; set; }
        public DbSet<Models.Database.PriceChange> PriceChanges { get; set; }
        public DbSet<Models.Database.Process> Processes { get; set; }
        public DbSet<Models.Database.ProcessTranslation> ProcessTranslations { get; set; }
        public DbSet<Models.Database.QuestionAndAnswer> QuestionsAndAnswers { get; set; }
        public DbSet<Models.Database.QuestionAndAnswerTranslation> QuestionsAndAnswerTranslations { get; set; }
        public DbSet<Models.Database.Quotation> Quotations { get; set; }
        public DbSet<Models.Database.QuotationChange> QuotationChanges { get; set; }
        public DbSet<Models.Database.QuotationItem> QuotationItems { get; set; }
        public DbSet<Models.Database.QuotationItemExtra> QuotationItemExtras { get; set; }
        public DbSet<Models.Database.QuotationItemPickupReturnTemporaryTax> QuotationItemPickupReturnTemporaryTaxTaxes { get; set; }
        public DbSet<Models.Database.QuotationItemService> QuotationItemServices { get; set; }
        public DbSet<Models.Database.ReasonToChooseUs> ReasonsToChooseUs { get; set; }
        public DbSet<Models.Database.ReasonToChooseUsTranslation> ReasonToChooseUsTranslations { get; set; }
        public DbSet<Models.Database.Reservation> Reservations { get; set; }
        public DbSet<Models.Database.ReservationChange> ReservationChanges { get; set; }
        public DbSet<Models.Database.ReservationExtra> ReservationExtras { get; set; }
        public DbSet<Models.Database.ReservationExtraDriver> ReservationExtraDrivers { get; set; }
        public DbSet<Models.Database.ReservationService> ReservationServices { get; set; }
        public DbSet<Models.Database.ReservationPickupReturnTemporaryTax> ReservationPickupReturnTemporaryTaxes { get; set; }
        public DbSet<Models.Database.ReservationQuotationCancellationReason> ReservationQuotationCancellationReasons { get; set; }
        public DbSet<Models.Database.Season> Seasons { get; set; }
        public DbSet<Models.Database.SeasonChange> SeasonChanges { get; set; }
        public DbSet<Models.Database.SeasonCategory> SeasonCategories { get; set; }
        public DbSet<Models.Database.Service> Services { get; set; }
        public DbSet<Models.Database.ServiceTranslation> ServiceTranslations { get; set; }
        public DbSet<Models.Database.Setting> Settings { get; set; }
        public DbSet<Models.Database.SocialNetwork> SocialNetworks { get; set; }
        public DbSet<Models.Database.TeamMember> TeamMembers { get; set; }
        public DbSet<Models.Database.TeamMemberTranslation> TeamMemberTranslations { get; set; }
        public DbSet<Models.Database.Testimonial> Testimonials { get; set; }
        public DbSet<Models.Database.TestimonialTranslation> TestimonialTranslations { get; set; }
        public DbSet<Models.Database.TranslatableSetting> TranslatableSettings { get; set; }
        public DbSet<Models.Database.TranslatableSettingTranslation> TranslatableSettingTranslations { get; set; }
        public DbSet<Models.Database.Voucher> Vouchers { get; set; }
        public DbSet<Models.Database.VoucherExtra> VoucherExtras { get; set; }

        public DbSet<Models.Database.ProcessedFile> ProcessedFiles { get; set; }
        public DbSet<Models.Database.ViaVerdeMovement> ViaVerdeMovements { get; set; }
    }
}
