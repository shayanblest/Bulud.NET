# Module ownership map

| Current responsibility | Target owner |
| --- | --- |
| Generic result types, shared exceptions, generic collection/expression helpers | `Bulud.Core` |
| File service contract | `Bulud.FileStorage.Abstractions` |
| Local and S3 file implementations | Their respective file-storage provider packages |
| Email and SMS contracts | `Bulud.Communication.Abstractions` |
| Email and Kavenegar implementations | Their respective communication provider packages |
| OTP contract | `Bulud.Security.Otp.Abstractions` |
| In-memory OTP implementation | `Bulud.Security.Otp.InMemory` |
| Exporter contracts | `Bulud.Exporting.Abstractions` |
| Export manager | `Bulud.Exporting` |
| CSV and PDF exporters | Their respective exporting provider packages |
| Repository/query infrastructure and UTC conversion | `Bulud.EntityFrameworkCore` |
| Middleware, authorization, claims, form files, logging/error configuration | `Bulud.AspNetCore` |
| JWT settings and integration | `Bulud.Authentication.Jwt` |

`BaseEntity` belongs in `Bulud.Core` only while it remains a persistence-agnostic primitive; otherwise it moves to `Bulud.EntityFrameworkCore`.
