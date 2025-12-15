using System.ComponentModel.DataAnnotations;
using Documan.Services;
using Microsoft.AspNetCore.Mvc;
using Propman.Constants;
using Propman.Logic;
using Propman.Repository;
using Propman.Services;
using Propman.Services.UserContext;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;
using PropMan.Services.AuditLogService;


namespace PropMan.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propRepo;
        private readonly IUserContext _userContext;
        private readonly IUserRepository _userRepo;
        private readonly IAuditLogService _auditLog;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IPropertyAssignmentRepository _propassRepo;
        private readonly ITenantRepository _tenantRepo;
        private readonly IPropAssLogic _propAssLogic;
        private readonly IEmailService _email;
        private readonly INotificationService _notify;

        public PropertyService(
            IPropertyRepository propRepo,
            IUserContext userContext,
            IUserRepository userRepo,
            IAuditLogService auditLog,
            IInvoiceRepository invoiceRepo,
            IPropertyAssignmentRepository propassRepo,
            ITenantRepository tenantRepo,
            IPropAssLogic propAssLogic,
            IEmailService email,
            INotificationService notify
            )
        {
            _propRepo = propRepo;
            _userContext = userContext;
            _userRepo = userRepo;
            _auditLog = auditLog;
            _invoiceRepo = invoiceRepo;
            _propassRepo = propassRepo;
            _tenantRepo = tenantRepo;
            _propAssLogic = propAssLogic;
            _email = email;
            _notify = notify;
        }

        public async Task<ApiResponse<IActionResult>> CreateProperty(CreateProperty request)
        {
            try
            {
                if (await _propRepo.PropertyExists(request.PropertyName))
                    return new ApiResponse<IActionResult>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.propexists,
                    };


                var compId = Guid.Parse(_userContext.CompanyId);
                var user = await _userRepo.GetUser(_userContext.UserName);

                var property = new Property
                {
                    PropertyName = request.PropertyName,
                    PropertyType = request.PropertyType,
                    Location = request.Location,
                    IsActive = true,
                    CompanyId = compId,
                    CostPrice = request.CostPrice,
                    SellingPrice = request.SellingPrice,
                    TotalUnits = request.TotalUnits,
                    OccupancyStatus = request.OccupancyStatus,
                    OccuppiedUnits = 0,
                    CreatedBy = user.UserId,
                    VacantUnits = request.TotalUnits,
                    UpdatedAt = DateTime.UtcNow
                };

                await _propRepo.AddProperty(property);


                if (request.Pictures != null && request.Pictures.Any())
                {
                    var uploadDir = Path.Combine("wwwroot", "uploads", "properties", $"property_{property.PropertyId}");
                    Directory.CreateDirectory(uploadDir);

                    var pictures = new List<Picture>();

                    foreach (var file in request.Pictures)
                    {
                        if (file.Length > 0)
                        {
                            var fileExt = Path.GetExtension(file.FileName);
                            var fileName = $"{Guid.NewGuid()}{fileExt}";
                            var filePath = Path.Combine(uploadDir, fileName);


                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            var baseUrl = "http://localhost:5152";

                            pictures.Add(new Picture
                            {
                                PropertyId = property.PropertyId,
                                FileName = fileName,
                                FilePath = $"{baseUrl}/uploads/properties/property_{property.PropertyId}/{fileName}"
                            });
                        }
                    }

                    // ✅ Save all pictures once (outside the loop)
                    if (pictures.Any())
                    {
                        await _propRepo.AddPicture(pictures);
                    }
                }


                // Log audit
                await _auditLog.Log(property.PropertyId, $"{user.Name} created a property");

                return new ApiResponse<IActionResult>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.propsuccess,
                };
            }
            catch (Exception ex)
            {

                return new ApiResponse<IActionResult>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }

        }

        public async Task<ApiResponse<List<Property>>> GetAllProps()
        {
            try
            {
                var result = await _propRepo.GetAllProperties();

                return new ApiResponse<List<Property>>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.propretSuccess,
                    Data = result

                };

            }
            catch (Exception ex)
            {

                return new ApiResponse<List<Property>>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message

                };
            }



        }
        public async Task<ApiResponse<Property>> DeleteProperty(Guid propid)
        {
            try
            {
                var user = await _userRepo.GetUser(_userContext.UserName);
                var property = await _propRepo.GetPropertyById(propid);
                var compId = Guid.Parse(_userContext.CompanyId);
                if (property == null || property.CompanyId != compId)
                {
                    return new ApiResponse<Property>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.propnotfound
                    };
                }
                var propertyactive = await _propassRepo.PropertyActive(propid);
                if (propertyactive)
                {
                    return new ApiResponse<Property>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.propassigned
                    };
                }
                var invoiceunpaid = await _invoiceRepo.PropertyInvoiceUnpaid(propid);
                if (invoiceunpaid)
                {
                    return new ApiResponse<Property>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.propinvoiceunpaid
                    };
                }

                property.IsActive = false;
                property.UpdatedAt = DateTime.UtcNow;
                await _propRepo.Delete(property);
                await _auditLog.Log(
                    property.PropertyId,
                    $"{user.Name} has deleted a property"
                );
                return new ApiResponse<Property>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.propdelsuccess
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Property>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }


        }

        public async Task<ApiResponse<Property>> UpdateProperty(UpdateProperty request)
        {
            try
            {
                var property = await _propRepo.GetPropertyById(request.PropertyId);
                if (property == null)
                {
                    return new ApiResponse<Property>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.propnotfound
                    };
                }
                property.PropertyName = request.PropertyName ?? property.PropertyName;
                property.PropertyType = request.PropertyType ?? property.PropertyType;
                property.Location = request.Location ?? property.Location;
                property.TotalUnits = request.TotalUnits > 0 ? request.TotalUnits : property.TotalUnits;
                property.UpdatedAt = DateTime.UtcNow;
                property.CostPrice = request.CostPrice ?? property.CostPrice;
                property.SellingPrice = request.SellingPrice ?? property.SellingPrice;

                await _propRepo.UpdateProperty(property);

                if (request.NewPictures != null && request.NewPictures.Any())
                {
                    var uploadDir = Path.Combine("wwwroot", "uploads", "properties", $"property_{property.PropertyId}");
                    Directory.CreateDirectory(uploadDir);

                    var pictures = new List<Picture>();
                    foreach (var file in request.NewPictures)
                    {
                        if (file.Length > 0)
                        {
                            var fileExt = Path.GetExtension(file.FileName);
                            var fileName = $"{Guid.NewGuid()}{fileExt}";
                            var filePath = Path.Combine(uploadDir, fileName);

                            using var stream = new FileStream(filePath, FileMode.Create);
                            await file.CopyToAsync(stream);

                            pictures.Add(new Picture
                            {
                                PropertyId = property.PropertyId,
                                FileName = fileName,
                                FilePath = $"/uploads/properties/property_{property.PropertyId}/{fileName}",
                                DateUploaded = DateTime.UtcNow
                            });
                        }
                    }

                    await _propRepo.AddPicture(pictures);
                }


                if (request.PicturesToRemove != null && request.PicturesToRemove.Any())
                {
                    await _propRepo.RemovePicturesAsync(request.PicturesToRemove);
                }


                var user = await _userRepo.GetUser(_userContext.UserName);
                await _auditLog.Log(property.PropertyId, $"{user.Name} updated property details");



                return new ApiResponse<Property>
                {
                    Status = (int)StatusCode.Success,
                    Message = "Property updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Property>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }


        }
        public async Task<ApiResponse<PropertyUnit>> AddUnit(CreateUnit request)
        {
            try
            {
                var property = await _propRepo.GetPropertyById(request.PropertyId);
                var user = await _userRepo.GetUser(_userContext.UserName);
                var compId = Guid.Parse(_userContext.CompanyId);

                var unitexists = await _propRepo.UnitExists(request.UnitName);
                var unitsfull = await _propRepo.UnitsFull(request.PropertyId);
                if (property == null || property.CompanyId != compId)
                {
                    return new ApiResponse<PropertyUnit>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.propnotfound
                    };
                }
                if (property.IsActive == false)
                {
                    return new ApiResponse<PropertyUnit>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.propnotfound
                    };
                }
                if (unitexists)
                {
                    return new ApiResponse<PropertyUnit>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.unitexists
                    };
                }
                if (unitsfull)
                {
                    return new ApiResponse<PropertyUnit>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.unitallowed
                    };
                }
                var propertyunit = new PropertyUnit
                {
                    PropertyId = request.PropertyId,
                    CompanyId = compId,
                    UnitName = request.UnitName,
                    RentPrice = request.RentPrice,
                    Description = request.Description,
                    Status = request.Status,
                    IsActive= true

                };

                await _propRepo.AddUnit(propertyunit);
                property.OccuppiedUnits++;
                property.VacantUnits--;
                property.UpdatedAt = DateTime.UtcNow;
                await _propRepo.UpdateProperty(property);
                await _auditLog.Log(
                   propertyunit.UnitId,

                   $"{user.Name} has added a unit to {property.PropertyName}"
               );
                return new ApiResponse<PropertyUnit>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.unitcreate

                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PropertyUnit>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message

                };
            }

        }

        public async Task<ApiResponse<Repair>> AddRepair(Repairreq request)
        {
            try
            {
                
                var property = await _propRepo.GetPropertyById(request.PropertyId);
                var propexists = await _propRepo.PropertyExists(property!.PropertyName);
                if (!propexists)
                {
                    return new ApiResponse<Repair>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.propexists

                    };
                }
                var unit = await _propRepo.GetUnitById(request.PropertyId);
                if (unit == null || unit.PropertyId != property.PropertyId)
                {
                    return new ApiResponse<Repair>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.unitunavailable

                    };
                }
                var repair = new Repair
                {
                    PropertyId = request.PropertyId,
                    CompanyId = Guid.Parse(_userContext.CompanyId!),
                    Description = request.Description,
                    Status = RepairStatus.Pending,
                    Cost = request.Cost ?? 0,
                    CompletedDate = DateTime.UtcNow,




                };
                await _propRepo.AddRepair(repair);
                return new ApiResponse<Repair>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.repairrecord

                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Repair>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message

                };
            }



        }
        public async Task<ApiResponse<Repair>> CompleteRepair(RepairComplete request)
        {
            try
            {
                var propertyTenant = await _propassRepo.GetById(request.PropertyTenantId);
                var propId = propertyTenant!.PropertyId;
                var property = await _propRepo.GetPropertyById(propId);
                var user = _userContext.UserName;
                if (property == null)
                {
                    return new ApiResponse<Repair>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.propnotfound
                    };
                }
                var repair = await _propRepo.GetRepairById(request.RepairId);

                repair.Status = request.Status;
                repair.CompletedDate = request.CompletedDate;

                await _propRepo.UpdateRepair(repair);
                await _auditLog.Log(
                    repair.RepairId,
                    $"{user} marked a repair as complete"
                );
                return new ApiResponse<Repair>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.repairupdate
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Repair>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }

        }

        public async Task<ApiResponse<PropertyTenant>> AssignProperty(PropTen request)
        {
            try
            {
                var property = await _propRepo.GetPropertyById(request.PropertyId);
                PropertyUnit? unit = null;
                var units = await _propRepo.GetUnitById(request.UnitId ?? Guid.Empty);
                if (request.UnitId != null && request.UnitId != Guid.Empty)
                {
                    unit = await _propRepo.GetUnitById((Guid)request.UnitId);
                    if (unit == null)
                    {
                        return new ApiResponse<PropertyTenant>
                        {
                            Status = (int)StatusCode.ValidationError,
                            Message = "Specified unit not found."
                        };
                    }

                    if (unit.Status == "Occupied")
                    {
                        return new ApiResponse<PropertyTenant>
                        {
                            Status = (int)StatusCode.ValidationError,
                            Message = "This unit is already occupied."
                        };
                    }
                }


                var tenant = await _tenantRepo.GetById(request.TenantId);
                if (tenant == null || tenant.IsActive == false)
                {
                    return new ApiResponse<PropertyTenant>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.tennotfound

                    };
                }
                var user = _userContext.UserName;
                if (property!.PropertyType == ConstantVariable.wp)
                {

                    // Save to database
                    var propertyTenant = new PropertyTenant
                    {
                        PropertyId = request.PropertyId,
                        TenantId = request.TenantId,
                        CompanyId = Guid.Parse(_userContext.CompanyId!),
                        UnitId = request.UnitId == Guid.Empty ? null : request.UnitId,
                        PlanId = request.PlanId,
                        StartDate = request.StartDate,
                        EndDate = request.StartDate.AddMonths(request.DurationMonths),
                        Status = ConstantVariable.Active ,
                        IsPrimaryTenant = request.IsPrimaryTenant,
                        InitialDeposit = request.InitialDeposit,
                        LastPaymentDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                    };

                    await _propassRepo.AddPropertyTenant(propertyTenant);
                    var proptenant = propertyTenant.PropertyTenantId;

                    var result = await _propAssLogic.WorkAndPay(
                        request.PropertyId,
                        request.InitialDeposit,
                        proptenant,
                        request.PlanId, request.StartDate,
                        propertyTenant.EndDate
                        );
                    propertyTenant.AmountDue = result.outstanding;
                    propertyTenant.installmentAmount = result.installmentAmount;
                    propertyTenant.totalInstallments = result.totalInstallments;
                    await _propassRepo.UpdatePropertyTenant(propertyTenant);
                    property.OccupancyStatus = "Occupied";
                    await _propRepo.UpdateProperty(property);

                    await _auditLog.Log(
                        propertyTenant.PropertyTenantId,
                        $"{user} assigned {tenant.FullName} to a property"
                    );
                    await _email.SendEmail(

     tenant.Email,
     "Work and Pay Property Assignment",
    $"Dear {tenant.FullName}, you’ve been successfully assigned to {property.PropertyName} under the Work and Pay plan. Please check your account for payment details."
);
                    await _notify.AddNotification(Guid.Parse(_userContext.CompanyId), propertyTenant.PropertyTenantId, "Email", $"Dear {tenant.FullName}, you’ve been successfully assigned to {property.PropertyName} under the Work and Pay plan. Please check your account for payment details.", tenant.TenantId);

                }




                if (property.PropertyType == "Rent")
                {
                    var propertyTenant = new PropertyTenant
                    {
                        PropertyTenantId = Guid.NewGuid(),
                        PropertyId = request.PropertyId,
                        UnitId = request.UnitId == Guid.Empty ? null : request.UnitId,

                        TenantId = request.TenantId,
                        CompanyId = Guid.Parse(_userContext.CompanyId),
                        PlanId = request.PlanId,
                        StartDate = request.StartDate,
                        EndDate = request.StartDate.AddMonths(request.DurationMonths),
                        Status = ConstantVariable.Active,
                        IsPrimaryTenant = request.IsPrimaryTenant,
                    };
                    await _propassRepo.AddPropertyTenant(propertyTenant);
                    await _propAssLogic.RentAssignment(request.PlanId, propertyTenant.TenantId, propertyTenant.UnitId ?? Guid.Empty, propertyTenant.PropertyTenantId);
                    if (unit != null)
                    {
                        unit.Status = "Occupied";
                        await _propRepo.UpdateUnit(unit);
                    }
                    else
                    {
                        property.OccupancyStatus = "Occupied";
                        await _propRepo.UpdateProperty(property);
                    }
                    await _email.SendEmail(

     tenant.Email,
     "Rental Property Assignment",
    $"Dear {tenant.FullName}, you’ve been successfully assigned to {property.PropertyName} under the Work and Pay plan. Please check your account for payment details."
);


                    await _auditLog.Log(
                        propertyTenant.PropertyTenantId,
                        $"{user} assigned {tenant.FullName} to a property"
                    );
                }
                property.OccupancyStatus = "Occupied";
                await _propRepo.UpdateProperty(property);


                return new ApiResponse<PropertyTenant>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.tenantassigned
                };


            }
            catch (Exception ex)
            {
                return new ApiResponse<PropertyTenant>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }


        }
        public async Task<ApiResponse<PropertyTenant>> UnassignProperty(Guid propertyTenantId)
        {
            try
            {
                var propertyTenant = await _propassRepo.GetById(propertyTenantId);
                if (propertyTenant == null)
                {
                    return new ApiResponse<PropertyTenant>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = "Property-Tenant record not found."
                    };
                }

                var tenant = await _tenantRepo.GetById(propertyTenant.TenantId);
                var property = await _propRepo.GetPropertyById(propertyTenant.PropertyId);
                var unit = await _propRepo.GetUnitById(propertyTenant.UnitId ?? Guid.Empty);

                if (property == null || unit == null)
                {
                    return new ApiResponse<PropertyTenant>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = "Property or Unit not found."
                    };
                }

                // Update relationships
                propertyTenant.Status = "Inactive";
                propertyTenant.EndDate = DateTime.UtcNow;
                propertyTenant.IsActive = false;
                unit.Status = "Vacant";
                property.OccupancyStatus = "Vacant";

                await _propassRepo.UpdatePropertyTenant(propertyTenant);
                await _propRepo.UpdateUnit(unit);
                await _propRepo.UpdateProperty(property);

                // Audit log
                var user = _userContext.UserName;
                await _auditLog.Log(
                    propertyTenant.PropertyTenantId,
                    $"{user} unassigned {tenant?.FullName ?? "a tenant"} from {property.PropertyName}"
                );

                return new ApiResponse<PropertyTenant>
                {
                    Status = (int)StatusCode.Success,
                    Message = "Tenant successfully unassigned from property.",
                    Data = propertyTenant
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PropertyTenant>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponse<PropertyUnit>> UpdateUnit(UpdateUnit request)
        {
            try
            {
                var propertyUnit = await _propRepo.GetUnitById(request.UnitId);
                if (propertyUnit == null)
                {
                    return new ApiResponse<PropertyUnit>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = "Unit not found."
                    };
                }

                var property = await _propRepo.GetPropertyById(propertyUnit.PropertyId);
                var user = await _userRepo.GetUser(_userContext.UserName);
                var compId = Guid.Parse(_userContext.CompanyId);

                if (property.CompanyId != compId)
                {
                    return new ApiResponse<PropertyUnit>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = "You are not authorized to update this unit."
                    };
                }

                // Update unit fields
                propertyUnit.UnitName = request.UnitName ?? propertyUnit.UnitName;
                propertyUnit.RentPrice = request?.RentPrice ?? propertyUnit.RentPrice;
                propertyUnit.Description = request.Description ?? propertyUnit.Description;
                propertyUnit.Status = request.Status ?? propertyUnit.Status;

                await _propRepo.UpdateUnit(propertyUnit);

                await _auditLog.Log(
                    propertyUnit.UnitId,
                    $"{user.Name} updated unit {propertyUnit.UnitName} for property {property.PropertyName}"
                );

                return new ApiResponse<PropertyUnit>
                {
                    Status = (int)StatusCode.Success,
                    Message = "Unit updated successfully.",
                    Data = propertyUnit
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PropertyUnit>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponse<PropertyUnit>> DeleteUnit(Guid unitId)
        {
            try
            {
                var propertyUnit = await _propRepo.GetUnitById(unitId);
                if (propertyUnit == null)
                {
                    return new ApiResponse<PropertyUnit>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = "Unit not found."
                    };
                }

                var property = await _propRepo.GetPropertyById(propertyUnit.PropertyId);
                var user = await _userRepo.GetUser(_userContext.UserName);
                var compId = Guid.Parse(_userContext.CompanyId);

                if (property.CompanyId != compId)
                {
                    return new ApiResponse<PropertyUnit>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = "You are not authorized to delete this unit."
                    };
                }




                await _propRepo.DeleteUnit(propertyUnit);


                property.TotalUnits--;
                property.VacantUnits = property.VacantUnits > 0 ? property.VacantUnits - 1 : 0;
                property.UpdatedAt = DateTime.UtcNow;
                await _propRepo.UpdateProperty(property);

                await _auditLog.Log(
                    propertyUnit.UnitId,
                    $"{user.Name} deleted unit {propertyUnit.UnitName} from {property.PropertyName}"
                );

                return new ApiResponse<PropertyUnit>
                {
                    Status = (int)StatusCode.Success,
                    Message = "Unit deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PropertyUnit>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponse<PropertyTenant>> AssignUnit(AssignUnit request)
        {
            try
            {
                var propertyTenant = await _propassRepo.GetById(request.PropertyTenantId);
                if (propertyTenant == null)
                {
                    return new ApiResponse<PropertyTenant>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = "Property tenant record not found."
                    };
                }

                var unit = await _propRepo.GetUnitById(request.UnitId);
                if (unit == null)
                {
                    return new ApiResponse<PropertyTenant>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = "Unit not found."
                    };
                }

                if (unit.Status == "Occupied")
                {
                    return new ApiResponse<PropertyTenant>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = "This unit is already occupied."
                    };
                }

                // Assign the unit
                propertyTenant.UnitId = request.UnitId;
                await _propassRepo.UpdatePropertyTenant(propertyTenant);

                // Update the unit’s status
                unit.Status = "Occupied";
                await _propRepo.UpdateUnit(unit);


                var property = await _propRepo.GetPropertyById(propertyTenant.PropertyId);
                property.OccuppiedUnits++;
                property.VacantUnits--;
                await _propRepo.UpdateProperty(property);


                await _auditLog.Log(
                    propertyTenant.PropertyTenantId,
                    $"{_userContext.UserName} assigned unit {unit.UnitName} to tenant {propertyTenant.TenantId}"
                );

                return new ApiResponse<PropertyTenant>
                {
                    Status = (int)StatusCode.Success,
                    Message = "Unit successfully assigned.",
                    Data = propertyTenant
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PropertyTenant>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponse<DashboardSummary>> GetDashboardSummary()
        {
            try
            {
                var result = await _propRepo.GetDashboardSummary();
                return new ApiResponse<DashboardSummary>
                {
                    Status = (int)StatusCode.Success,
                    Data = result
                };
            }
            catch (Exception ex)
            {

                return new ApiResponse<DashboardSummary>
                {
                    Status = (int)StatusCode.Success,
                    Message = ex.Message
                };
            }
        }


        public async Task<ApiResponse<FetchDataRecords<TenantPropertyDto>>> GetTenantProperties(Guid tenantId, int page, int pageSize)
        {
            var result = await _propRepo.GetPropertiesByTenantId(tenantId, page, pageSize);


            return new ApiResponse<FetchDataRecords<TenantPropertyDto>>
            {
                Status = (int)StatusCode.Success,
                Message = "Success",
                Data = result
            };
        }
        public async Task<ApiResponse<FetchDataRecords<AllUnits>>>GetUnitsById(Guid propId,int page,int pageSize)
        {
            var result = await _propRepo.GetUnitsByPropertyId(propId,page,pageSize);
            return new ApiResponse<FetchDataRecords<AllUnits>>
            {
                Status = (int)StatusCode.Success,
                Message = "Success",
                Data = result
            };
        }
        public async Task<ApiResponse<List<UnitDto>>>GetUnitsByPropertyId(Guid propId)
        {
            var res = await _propRepo.GetUnitsByPropertyId(propId);

            return new ApiResponse<List<UnitDto>>
            {
                Status=(int)StatusCode.Success,
                Message = "Retrieved",
                Data= res
            };
        }
        public async Task<ApiResponse<List<PropertyDto>>>PropertiesdByPropertyId()
        {
            var res = await _propRepo.PropertiesByPropertyId();

            return new ApiResponse<List<PropertyDto>>
            {
                Status=(int)StatusCode.Success,
                Message = "Retrieved",
                Data= res
            };
        }
        public async Task<ApiResponse<FetchDataRecords<RepairDto>>>RepairsByPropertyId(Guid propId,int page,int pageSize,DateTime? startDate,DateTime? endDate)
        {
            var result = await _propRepo.RepairsByPropertyId(propId,page,pageSize,startDate,endDate);

            return new ApiResponse<FetchDataRecords<RepairDto>>
            {
                Status=(int)StatusCode.Success,
                Message = "Retrieved",
                Data= result
            };
        }
        public async Task<ApiResponse<FetchDataRecords<AllRepairs>>>AllRepairs(int page,int pageSize,DateTime? startDate,DateTime? endDate,string? status)
        {
            var result = await _propRepo.AllRepairs(page,pageSize,startDate,endDate,status);

            return new ApiResponse<FetchDataRecords<AllRepairs>>
            {
                Status=(int)StatusCode.Success,
                Message = "Retrieved",
                Data= result
            };
        }
    }
}










    

    
