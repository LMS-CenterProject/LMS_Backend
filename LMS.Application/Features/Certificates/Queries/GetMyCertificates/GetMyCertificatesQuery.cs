using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Certificates.Queries.GetMyCertificates
{
    public sealed record GetMyCertificatesQuery
    : IRequest<Result<IEnumerable<CertificateDto>>>;
}
