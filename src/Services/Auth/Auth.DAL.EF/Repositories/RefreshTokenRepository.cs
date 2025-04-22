using Auth.DAL.EF.Models;
using Auth.Domain.Models;
using Auth.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Auth.DAL.EF.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthDbContext _context;
    private readonly IMapper _mapper; 

    public RefreshTokenRepository(AuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RefreshToken> GetToken(string token)
    {
        var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.token == token && t.revoked == false); 
        var refreshToken = _mapper.Map<RefreshToken>(tokenEntity);
        return refreshToken;
    }

    public async Task<IEnumerable<RefreshToken>> GetUserTokens(Guid userId)
    {
        var tokenEntities = await _context.RefreshTokens.Where(t => t.user_id == userId && t.revoked == false).ToListAsync();
        var refreshTokens = _mapper.Map<IEnumerable<RefreshToken>>(tokenEntities);
        return refreshTokens;
    }

    public async Task RevokeToken(string token)
    {
        var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.token == token);
        if (existingToken != null) {
            existingToken.revoked = true;
            existingToken.used = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveToken(RefreshToken token)
    {
        var tokenEntity = _mapper.Map<RefreshTokenEntity>(token);
        await _context.RefreshTokens.AddAsync(tokenEntity);
        await _context.SaveChangesAsync();
    }
}