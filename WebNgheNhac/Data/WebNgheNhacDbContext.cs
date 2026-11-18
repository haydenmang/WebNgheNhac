using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebNgheNhac.Models;
using Type = WebNgheNhac.Models.Type;

namespace WebNgheNhac.Data;

public partial class WebNgheNhacDbContext : DbContext
{
    public WebNgheNhacDbContext()
    {
    }

    public WebNgheNhacDbContext(DbContextOptions<WebNgheNhacDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistSong> PlaylistSongs { get; set; }

    public virtual DbSet<Singer> Singers { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Albums__3214EC0788B8D740");

            entity.Property(e => e.AlbumName).HasMaxLength(100);
            entity.Property(e => e.CoverUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Singer).WithMany(p => p.Albums)
                .HasForeignKey(d => d.SingerId)
                .HasConstraintName("FK_Albums_Singers");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Playlist__3214EC07AD077EAB");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Playlists_Users");
        });

        modelBuilder.Entity<PlaylistSong>(entity =>
        {
            entity.HasKey(e => new { e.PlaylistId, e.SongId });

            entity.ToTable("Playlist_Songs");

            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Playlist).WithMany(p => p.PlaylistSongs)
                .HasForeignKey(d => d.PlaylistId)
                .HasConstraintName("FK_PlaylistSongs_Playlists");

            entity.HasOne(d => d.Song).WithMany(p => p.PlaylistSongs)
                .HasForeignKey(d => d.SongId)
                .HasConstraintName("FK_PlaylistSongs_Songs");
        });

        modelBuilder.Entity<Singer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Singers__3214EC07529DC8BB");

            entity.Property(e => e.Avatar).HasMaxLength(500);
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Songs__3214EC07BAE53705");

            entity.Property(e => e.AudioUrl).HasMaxLength(500);
            entity.Property(e => e.CoverUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Album).WithMany(p => p.Songs)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Songs_Albums");

            entity.HasOne(d => d.Singer).WithMany(p => p.Songs)
                .HasForeignKey(d => d.SingerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Songs_Singers");

            entity.HasOne(d => d.Type).WithMany(p => p.Songs)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Songs_Types");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Types__3214EC07CE773AB5");

            entity.HasIndex(e => e.TypeName, "UQ__Types__D4E7DFA8F5E132BB").IsUnique();

            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07FB7F50B4");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E491C358EB").IsUnique();

            entity.Property(e => e.Avatar)
                .HasMaxLength(500)
                .HasDefaultValue("/images/default-avatar.png");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
