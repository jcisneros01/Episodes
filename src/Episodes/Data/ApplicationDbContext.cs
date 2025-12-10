using System;
using System.Collections.Generic;
using Episodes.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Episodes.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<episode> episodes { get; set; }

    public virtual DbSet<genre> genres { get; set; }

    public virtual DbSet<season> seasons { get; set; }

    public virtual DbSet<show> shows { get; set; }

    public virtual DbSet<tv_data_provider> tv_data_providers { get; set; }

    public virtual DbSet<tv_network> tv_networks { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<user_show> user_shows { get; set; }

    public virtual DbSet<watched_episode> watched_episodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=episodes;Username=episodes_user;Password=episodes_password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<episode>(entity =>
        {
            entity.HasKey(e => e.id).HasName("episodes_pkey");

            entity.ToTable("episodes", "episodes");

            entity.HasIndex(e => new { e.season_id, e.episode_number }, "episodes_season_id_episode_number_key").IsUnique();

            entity.HasIndex(e => e.season_id, "episodes_season_id_idx");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.data_provider).WithMany(p => p.episodes)
                .HasForeignKey(d => d.data_provider_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("episodes_data_provider_id_fkey");

            entity.HasOne(d => d.season).WithMany(p => p.episodes)
                .HasForeignKey(d => d.season_id)
                .HasConstraintName("episodes_season_id_fkey");
        });

        modelBuilder.Entity<genre>(entity =>
        {
            entity.HasKey(e => e.id).HasName("genres_pkey");

            entity.ToTable("genres", "episodes");

            entity.HasIndex(e => e.name, "genres_name_key").IsUnique();

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasMany(d => d.shows).WithMany(p => p.genres)
                .UsingEntity<Dictionary<string, object>>(
                    "genre_show",
                    r => r.HasOne<show>().WithMany()
                        .HasForeignKey("show_id")
                        .HasConstraintName("genre_shows_show_id_fkey"),
                    l => l.HasOne<genre>().WithMany()
                        .HasForeignKey("genre_id")
                        .HasConstraintName("genre_shows_genre_id_fkey"),
                    j =>
                    {
                        j.HasKey("genre_id", "show_id").HasName("genre_shows_pkey");
                        j.ToTable("genre_shows", "episodes");
                        j.HasIndex(new[] { "show_id" }, "genre_shows_show_id_idx");
                    });
        });

        modelBuilder.Entity<season>(entity =>
        {
            entity.HasKey(e => e.id).HasName("seasons_pkey");

            entity.ToTable("seasons", "episodes");

            entity.HasIndex(e => e.show_id, "seasons_show_id_idx");

            entity.HasIndex(e => new { e.show_id, e.season_number }, "seasons_show_id_season_number_key").IsUnique();

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.data_provider).WithMany(p => p.seasons)
                .HasForeignKey(d => d.data_provider_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seasons_data_provider_id_fkey");

            entity.HasOne(d => d.show).WithMany(p => p.seasons)
                .HasForeignKey(d => d.show_id)
                .HasConstraintName("seasons_show_id_fkey");
        });

        modelBuilder.Entity<show>(entity =>
        {
            entity.HasKey(e => e.id).HasName("shows_pkey");

            entity.ToTable("shows", "episodes");

            entity.HasIndex(e => new { e.external_id, e.data_provider_id }, "shows_external_id_data_provider_id_key").IsUnique();

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.data_provider).WithMany(p => p.shows)
                .HasForeignKey(d => d.data_provider_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shows_data_provider_id_fkey");
        });

        modelBuilder.Entity<tv_data_provider>(entity =>
        {
            entity.HasKey(e => e.id).HasName("tv_data_providers_pkey");

            entity.ToTable("tv_data_providers", "episodes");

            entity.HasIndex(e => e.name, "tv_data_providers_name_key").IsUnique();

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<tv_network>(entity =>
        {
            entity.HasKey(e => e.id).HasName("tv_networks_pkey");

            entity.ToTable("tv_networks", "episodes");

            entity.HasIndex(e => e.name, "tv_networks_name_key").IsUnique();

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasMany(d => d.shows).WithMany(p => p.networks)
                .UsingEntity<Dictionary<string, object>>(
                    "network_show",
                    r => r.HasOne<show>().WithMany()
                        .HasForeignKey("show_id")
                        .HasConstraintName("network_shows_show_id_fkey"),
                    l => l.HasOne<tv_network>().WithMany()
                        .HasForeignKey("network_id")
                        .HasConstraintName("network_shows_network_id_fkey"),
                    j =>
                    {
                        j.HasKey("network_id", "show_id").HasName("network_shows_pkey");
                        j.ToTable("network_shows", "episodes");
                        j.HasIndex(new[] { "show_id" }, "network_shows_show_id_idx");
                    });
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.ToTable("users", "episodes");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.email).HasMaxLength(254);
            entity.Property(e => e.password_hash).HasMaxLength(255);
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<user_show>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.show_id }).HasName("user_shows_pkey");

            entity.ToTable("user_shows", "episodes");

            entity.HasIndex(e => e.show_id, "user_shows_show_id_idx");

            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.show).WithMany(p => p.user_shows)
                .HasForeignKey(d => d.show_id)
                .HasConstraintName("user_shows_show_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.user_shows)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("user_shows_user_id_fkey");
        });

        modelBuilder.Entity<watched_episode>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.episode_id }).HasName("watched_episodes_pkey");

            entity.ToTable("watched_episodes", "episodes");

            entity.HasIndex(e => e.episode_id, "watched_episodes_episode_id_idx");

            entity.Property(e => e.watched_at).HasDefaultValueSql("now()");

            entity.HasOne(d => d.episode).WithMany(p => p.watched_episodes)
                .HasForeignKey(d => d.episode_id)
                .HasConstraintName("watched_episodes_episode_id_fkey");

            entity.HasOne(d => d.user).WithMany(p => p.watched_episodes)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("watched_episodes_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
